using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Cinemachine;

public enum DecayEaseType { Exponential, Quartic }

//public static class InputActionExtensions
//{
//    public static bool Pressed(this InputAction inputAction)
//    {
//        return inputAction.IsPressed();
//    }

//    public static bool PressedThisFrame(this InputAction inputAction)
//    {
//        return inputAction.triggered && inputAction.WasPressedThisFrame();
//    }

//    public static bool ReleasedThisFrame(this InputAction inputAction)
//    {
//        return inputAction.triggered && inputAction.ReleasedThisFrame();
//    }
//}

public class PlayerController : MonoBehaviour, IDamagable
{
    public PlayerStats stats;
    [SerializeField] private bool paused = true;

    private IEnumerator rightPickup;

    [Header("Movement Confinements")]
    [SerializeField] private float maxYSpeed = 15f;
    [SerializeField] private float terminalVelocity = 25f;

    [Header("Gravity")]
    [SerializeField] private float gravity = 3f;
    [SerializeField] private float notHoldingJumpCounterForce = 10f;
    private float prevYPos;
    private float yDelta = 0f;
    [SerializeField] private float yDeltaThreshold = 0.05f;
    [SerializeField] private float rescueTime = 3f;
    private float currentRescueTime = 0f;

    [Header("Jump Ability")]
    [SerializeField] private Transform sphereOrigin;
    public Transform Origin { get => sphereOrigin; }
    [SerializeField] private Transform headOrigin;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float groundedSphereRadius = 0.3f;
    [SerializeField] private float coyoteTime = 0.2f;
    private float currentCoyoteTime;
    private bool canResetCoyote;
    private bool bouncedOffHead = false;
    private bool canJump { get => IsGrounded() || currentCoyoteTime > 0; }
    [SerializeField] private float jumpInputBuffer = 0.2f;
    private float currentJumpInputBuffer;
    [SerializeField] private float slopeLimit = 60f;
    [SerializeField] private float downRayDistance = 5f;


    [Header("Dash Ability")]
    [SerializeField] private Ability dashAbility;
    [SerializeField] private float dashDuration = 0.5f;
    private float currentDashDuration;
    private Vector3 dashDirection; //normalized dash direction
    [SerializeField] private ParticleSystem speedLines;

    [Header("Momentum Conversion")]
    [SerializeField] private DecayEaseType decayEase;
    [SerializeField] private float decayTimeStartBuffer = 0.2f;
    private float currentDecayTimeBuffer;
    [SerializeField] private float momentumDecayTime = 1.0f;
    private float momentumTime;
    [SerializeField][Range(0,180f)] private float momentumDecayAngleThreshold;
    [SerializeField] private float dashMomentumConversionRate = 0.5f;
    [SerializeField] private float momentumConversionBuffer = 0.2f;
    private float currentMomentumConversionBuffer;
    private float movementChangeAngle;
    private Vector3 momentumAllowedHorizontalDirection;
    private Vector3 prevMovement;
    private Vector3 startMomentum;

    [Header("GameEvents")]
    [SerializeField] private GameEvent interactibleChanged;
    [SerializeField] private GameEvent petChanged;

    [Header("Interact Ranges")]
    public float interactRange = 1f;
    public float pickupRange = 0.6f;
    public float petReach = 2f;
    public IInteractable ClosestInteractable { get => closestInteractable; }
    public IPickupable ClosestPickup { get => closestPickup; set => closestPickup = value; }
    public IPetable ClosestPet { get => closestPet; }

    private IInteractable closestInteractable;
    private IPickupable closestPickup;
    private IPetable closestPet;


    private Vector2 inputVector;

    //velocities
    private Vector3 movementVelocity; //player input times movementspeed
    private Vector3 momentumVelocity; //outside forces
    private Vector3 cappedVelocity; //movement + momentum that cant pass a certain magnitude (momentum has priority)
    private Vector3 dashVelocity; //dash operates outside of the capped velocity, allowing you to break it -> making the dash always dash, while preventing infinite speed gain
    private Vector3 totalVelocity; //capped + dash

    private Vector3 forceVector;
    private Vector3 startForceVector;
    [SerializeField] private float forceDuration = 0.5f;
    private float currentForceDuration;

    private Rigidbody playerRigidbody;
    private Collider playerCollider;

    private PlayerInputs playerInput; //new input system is a buggy little b****

    //singleton
    private static PlayerController instance;
    public static PlayerController Instance { get => instance; }

    private bool pauseGroundCheck = false;

    [SerializeField] private bool damagable;
    public bool Damagable { get => damagable; }

    [SerializeField] private Transform transformOverwrite;
    public Transform Transform { get => transformOverwrite ? transformOverwrite : transform; }

    public bool Dashing { get => currentDashDuration < dashDuration; }

    public GameEvent gameend;
    private CinemachineImpulseSource cameraImpulse;

    public MoneyInstance money;

    private void Awake()
    {
        playerInput = new PlayerInputs();

        //set singleton
        if (instance != null)
        {
            return;
        }
        instance = this;

        stats.FakeAwake(); //since normal SO awake doesn't work like monobehaviour awake
    }

    private void Start()
    {

        playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.useGravity = false;
        playerCollider = GetComponent<CapsuleCollider>();
        momentumTime = momentumDecayTime;
        prevYPos = transform.position.y;

        dashAbility.CurrentUses = dashAbility.maxUseCount;
        dashAbility.CurrentCooldown = 0;
        currentJumpInputBuffer = 0;
        currentCoyoteTime = 0;
        currentDecayTimeBuffer = 0;
        currentDashDuration = dashDuration;
        currentMomentumConversionBuffer = 0;
        currentRescueTime = 0;

        dashDirection = Vector3.zero;
    }

    private void FixedUpdate()
    {

    }

    private void Update()
    {
        if (paused) return;

        stats.Update();
        FindClosestInteractable(interactRange);
        FindClosestPickup(pickupRange);
        FindClosestPetable(petReach);

        if (closestPickup != null) closestPickup.PickUp();
        if (closestPet != null && Input.GetKey(KeyCode.P)) closestPet.Pet();
        if (closestInteractable != null && Input.GetKeyDown(KeyCode.E)) closestInteractable.Interact();

        if (Input.GetButtonDown("Jump"))
        {
            currentJumpInputBuffer = jumpInputBuffer;
        }

        //delayed grounded effects
        if (IsGrounded())
        {
            currentDecayTimeBuffer -= Time.deltaTime;

            if (canResetCoyote)
            {
                currentCoyoteTime = coyoteTime;
                canResetCoyote = false;
            }
        }
        else
        {
            currentDecayTimeBuffer = decayTimeStartBuffer;
            currentCoyoteTime -= Time.deltaTime;
            canResetCoyote = true;
        }

        ManageCooldowns();
        HandleGravity();
        HandleMovement();
        HandleDash();
        HandleJump();
        VelocityCalculation();
    }

    private void LateUpdate()
    {
        if (currentDashDuration >= dashDuration && IsGrounded()) currentMomentumConversionBuffer -= Time.deltaTime;
        currentDashDuration = Mathf.Clamp(currentDashDuration + Time.deltaTime, 0, dashDuration);
        currentForceDuration = Mathf.Clamp(currentForceDuration + Time.deltaTime, 0, forceDuration);
        currentJumpInputBuffer -= Time.deltaTime;
        prevYPos = transform.position.y;
    }

    public void TakeDamage(float damage)
    {
        stats.CalculateDamageTaken(damage);
        if(TryGetComponent(out cameraImpulse))
        {
            cameraImpulse.GenerateImpulse();
        }

        if (stats.CurrentHealth <= 0)
        { 
            Die();
        }
    }

    public void TakeDirectDamage(float damage)
    {
        stats.CalculateDamageTaken(damage, true); //There was a bool true in there causíng a bug, It it was important, add it again. 
        if (stats.CurrentHealth <= 0) Die();
    }

    private void Die()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        gameend.Raise();
    }

    private void HandleMovement()
    {
        //handle force
        forceVector = EaseInExponential(currentForceDuration, startForceVector, -startForceVector, forceDuration);

        //decay momentum when grounded or if the angle between the momentum direction and intended movement exeeds the threshold
        movementChangeAngle = Mathf.Abs(Vector3.Angle(prevMovement, momentumAllowedHorizontalDirection));
        if (IsGrounded() && !Dashing && currentDecayTimeBuffer <= 0 || movementChangeAngle > momentumDecayAngleThreshold && !Dashing && currentDecayTimeBuffer <= 0)
        {
            momentumTime = Mathf.Clamp(momentumTime + Time.deltaTime, 0, momentumDecayTime);
        }
       
        if (!Dashing) //if not dashing
        {
            float y = movementVelocity.y;
            movementVelocity = Input.GetButton("Sprint") ? (transform.right * inputVector.x + transform.forward * inputVector.y).normalized * stats.walkSpeedStat.TotalValue : (transform.right * inputVector.x + transform.forward * inputVector.y).normalized * stats.walkSpeedStat.TotalValue;
            movementVelocity.y = y;
        }
        else
        {
            momentumVelocity.y = 0f;
            movementVelocity = dashDirection * stats.walkSpeedStat.TotalValue;
        }

        //if the player is trying to change the movement vector cache it to compare the to the change threshold next frame
        if(inputVector.magnitude > 0)
        {
            prevMovement = movementVelocity;
        }

        //lose momentum if you are looking too far away from the current mometum direction
        if (momentumVelocity.magnitude > 0)
        {
            switch (decayEase)
            {
                case DecayEaseType.Exponential:
                    momentumVelocity = EaseInExponential(momentumTime, startMomentum, -startMomentum, momentumDecayTime);
                    break;
                case DecayEaseType.Quartic:
                    momentumVelocity = EaseInQuart(momentumTime, startMomentum, -startMomentum, momentumDecayTime);
                    break;
                default:
                    break;
            }
        }
    }

    private void HandleDash()
    {
        //decay dash
        dashVelocity = EaseInExponential(currentDashDuration, dashDirection * stats.dashStat.TotalValue, -dashDirection * stats.dashStat.TotalValue, dashDuration);

        if(Dashing && !speedLines.isPlaying)
        {
            speedLines.Play();
        }
        else if(speedLines.isPlaying && !Dashing)
        {
            speedLines.Stop();
        }

        //dash (todo: on tap)
        if (Input.GetButtonDown("Sprint") && dashAbility.CurrentUses > 0)
        {
            if (inputVector.magnitude > 0) dashDirection = transform.right * inputVector.x + transform.forward * inputVector.y;
            else dashDirection = transform.forward;

            dashVelocity = dashDirection * stats.dashStat.TotalValue;
            //AddMomentum(transform.forward, dashForce * dashMomentumConversionRate);
            RedirectMomentum(dashDirection);
            currentDashDuration = 0;
            currentMomentumConversionBuffer = momentumConversionBuffer;
            AkSoundEngine.PostEvent("Player_Dash", gameObject);

            dashAbility.UpdateUse();
        }
    }

    private void HandleJump()
    {
        //jumping
        if (currentJumpInputBuffer > 0 && canJump)
        {
            StartCoroutine(PauseGroundCheckForSeconds(0.2f));
            movementVelocity.y = stats.jumpStat.TotalValue;
            if (movementChangeAngle <= momentumDecayAngleThreshold) RedirectMomentum(Vector3.ProjectOnPlane(movementVelocity, Vector3.up));
            currentJumpInputBuffer = 0;
            currentCoyoteTime = 0;

            if (currentMomentumConversionBuffer > 0)
            {
                //converse momentum
                AddMomentum(transform.forward, stats.dashStat.TotalValue * dashMomentumConversionRate);
            }
            currentDashDuration = dashDuration;
        }
    }

    private IEnumerator PauseGroundCheckForSeconds(float time)
    {
        pauseGroundCheck = true;
        yield return new WaitForSeconds(time);
        pauseGroundCheck = false;
    }

    private void VelocityCalculation()
    {
        //check if magnitude of total horizontal velocity exeeds max and adjust accordingly
        cappedVelocity = movementVelocity + momentumVelocity;
        Vector3 cappedHorizontalVelocity = Vector3.ProjectOnPlane(cappedVelocity, Vector3.up);
        if (Dashing)
        {
            cappedVelocity.y = 0;
        }
        else if (cappedHorizontalVelocity.magnitude > stats.cappedHorizontalSpeedStat.TotalValue)
        {
            cappedHorizontalVelocity = cappedHorizontalVelocity.normalized * stats.cappedHorizontalSpeedStat.TotalValue;
            cappedVelocity.x = cappedHorizontalVelocity.x;
            cappedVelocity.z = cappedHorizontalVelocity.z;
        }

        //check total magnitued for seperate velocity cap that includes the dash
        totalVelocity = cappedVelocity + dashVelocity;
        Vector3 maxHorizontalVelocity = Vector3.ProjectOnPlane(totalVelocity, Vector3.up);
        if(maxHorizontalVelocity.magnitude > stats.maxHorizontalSpeedStat.TotalValue)
        {
            maxHorizontalVelocity = maxHorizontalVelocity.normalized * stats.maxHorizontalSpeedStat.TotalValue;
            totalVelocity.x = maxHorizontalVelocity.x;
            totalVelocity.z = maxHorizontalVelocity.z;
        }

        //check if total y-velocity exeeds max and adjust accordingly
        if (totalVelocity.y > maxYSpeed) totalVelocity.y = maxYSpeed * Mathf.Sign(totalVelocity.y);
        else if (-totalVelocity.y > terminalVelocity) totalVelocity.y = terminalVelocity * Mathf.Sign(totalVelocity.y);

        //finally set the rigidbodies velocity
        playerRigidbody.velocity = totalVelocity + forceVector;
    }

    private void HandleGravity()
    {
        if (!IsGrounded())
        {
            if (HittingHead() && !bouncedOffHead)
            {
                movementVelocity.y = -Mathf.Abs(movementVelocity.y);
                //movementVelocity.y = 0;
                bouncedOffHead = true;
            }

            yDelta = Mathf.Abs(transform.position.y - prevYPos);
            if(yDelta <= yDeltaThreshold * Time.deltaTime)
            {
                currentRescueTime += Time.deltaTime;
                if(currentRescueTime >= rescueTime)
                {
                    currentRescueTime = 0;
                    movementVelocity.y = stats.jumpStat.TotalValue;
                }
            }
            movementVelocity.y -= gravity * Time.deltaTime;
            if (!Dashing && !Input.GetButton("Jump"))
            {
                movementVelocity.y -= notHoldingJumpCounterForce * Time.deltaTime;
            }

        }
    }

    //todo: into ability scriptable obj
    private void ManageCooldowns()
    {
        if(dashAbility.CurrentUses < dashAbility.maxUseCount)
        {
            float cooldown = Mathf.Clamp(dashAbility.cooldown * (1 - stats.cooldownReduction.TotalValue), 0, dashAbility.cooldown);
            dashAbility.CurrentCooldown = Mathf.Clamp(dashAbility.CurrentCooldown + Time.deltaTime, 0, cooldown);
            if(dashAbility.CurrentCooldown == cooldown)
            {
                dashAbility.CurrentUses++;
                dashAbility.CurrentCooldown = 0;
            }
        }
    }

    public bool IsGrounded()
    {
        if (pauseGroundCheck) return false;

        float colliderDistanceToGround = Vector3.Distance(playerCollider.bounds.center, sphereOrigin.position);
        bool inContactWithGround = Physics.SphereCast(playerCollider.bounds.center, groundedSphereRadius, Vector3.down, out RaycastHit sphereCastHit, colliderDistanceToGround, groundLayers);

        return inContactWithGround && !OnSlope();
    }

    public bool IsActivelyMoving()
    {
        return inputVector.magnitude > 0;
    }

    public bool HittingHead()
    {
        if (pauseGroundCheck) return false;

        float colliderDistanceToHead = Vector3.Distance(playerCollider.bounds.center, headOrigin.position);
        bool inContactWithGround = Physics.SphereCast(playerCollider.bounds.center, groundedSphereRadius, Vector3.up, out RaycastHit sphereCastHit, colliderDistanceToHead, groundLayers);

        return inContactWithGround;
    }

    public bool OnSlope()
    {
        float angle = Mathf.Infinity;
        if (Physics.Raycast(sphereOrigin.position, Vector3.down, out RaycastHit hit, downRayDistance, groundLayers))
        {
            angle = Vector3.Angle(hit.normal, Vector3.up);
        }

        return angle >= slopeLimit;
    }

    public void Heal(float value)
    {
        stats.Heal(value);
    }

    #region ClosestInterfaceFinders

    public IInteractable FindClosestInteractable(float distance)
    {
        List<IInteractable> interactables = new List<IInteractable>();
        Collider[] colliders = Physics.OverlapSphere(sphereOrigin.position, distance);

        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.Interactable)
            {
                interactables.Add(interactable);
            }
        }

        if (interactables.Count <= 0)
        {
            if(closestInteractable != null)
            {
                closestInteractable = null;
                interactibleChanged.Raise();
            }
            return null;
        }

        float currentDistance;
        float smallestDistance = Mathf.Infinity;
        IInteractable closestToPlayer = null;

        foreach (IInteractable interactable in interactables)
        {
            currentDistance = Vector3.Distance(interactable.Transform.position, transform.position);
            if (smallestDistance > currentDistance)
            {
                closestToPlayer = interactable;
                smallestDistance = currentDistance;
            }
        }

        if (closestInteractable != closestToPlayer)
        {
            closestInteractable = closestToPlayer;
            interactibleChanged.Raise();
        }

        return closestToPlayer;
    }

    public IPetable FindClosestPetable(float distance)
    {
        List<IPetable> petables = new List<IPetable>();
        Collider[] colliders = Physics.OverlapSphere(sphereOrigin.position, distance);

        foreach (Collider collider in colliders)
        {
            IPetable pet = collider.GetComponent<IPetable>();
            if (pet != null && pet.Petable)
            {
                petables.Add(pet);
            }
        }

        if (petables.Count <= 0)
        {
            if(closestPet != null)
            {
                closestPet = null;
                petChanged.Raise();
            }
            return null;
        }

        float currentDistance;
        float smallestDistance = Mathf.Infinity;
        IPetable closestToPlayer = null;

        foreach (IPetable pet in petables)
        {
            currentDistance = Vector3.Distance(pet.TargetTransform.position, transform.position);
            if (smallestDistance > currentDistance)
            {
                closestToPlayer = pet;
                smallestDistance = currentDistance;
            }
        }

        if(closestPet != closestToPlayer)
        {
            closestPet = closestToPlayer;
            petChanged.Raise();
        }

        if (closestPet != closestToPlayer)
        {
            closestPet = closestToPlayer;
            petChanged.Raise();
        }

        return closestPet;
    }

    public IPickupable FindClosestPickup(float distance)
    {
        List<IPickupable> interactables = new List<IPickupable>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance);

        foreach (Collider collider in colliders)
        {
            IPickupable pickup = collider.GetComponent<IPickupable>();
            if (pickup != null && pickup.Pickupable)
            {
                interactables.Add(pickup);
            }
        }

        if (interactables.Count <= 0)
        {
            closestPickup = null;
            return null;
        }

        float currentDistance;
        float smallestDistance = Mathf.Infinity;
        IPickupable closestToPlayer = null;

        foreach (IPickupable interactable in interactables)
        {
            currentDistance = Vector3.Distance(interactable.TargetTransform.position, transform.position);
            if (smallestDistance > currentDistance)
            {
                closestToPlayer = interactable;
                smallestDistance = currentDistance;
            }
        }

        closestPickup = closestToPlayer;
        return closestToPlayer;
    }

    #endregion

    #region Input Callbacks

    public void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    //void OnJump(InputValue value)
    //{
    //    performJump = true;
    //}

    //void OnShiftAbility(InputValue value)
    //{
    //    performShiftAbility = true;
    //}

    #endregion

    #region Momentum

    public void AddMomentum(Vector3 direction, float force)
    {
        ChangeMomentum(momentumVelocity + direction.normalized * force, true);
    }

    public void RedirectMomentum(Vector3 newDirection)
    {
        ChangeMomentum(newDirection.normalized * momentumVelocity.magnitude);
    }

    public void ChangeMomentum(Vector3 newMomentum, bool resetDecay = false)
    {
        momentumVelocity = newMomentum;
        momentumAllowedHorizontalDirection = Vector3.ProjectOnPlane(momentumVelocity, Vector3.up).normalized;
        startMomentum = momentumVelocity;

        if(resetDecay) momentumTime = 0;
    }

    public void AddForce(Vector3 direction, float force)
    {
        ChangeForce(forceVector + direction.normalized * force, true);
    }

    public void RedirectForce(Vector3 newDirection)
    {
        ChangeForce(newDirection.normalized * forceVector.magnitude);
    }

    public void ChangeForce(Vector3 newForce, bool resetDecay = false)
    {
        //Vector3 newForce = Vector3.ProjectOnPlane(newMomentum, Vector3.up);
        if (newForce.magnitude < forceVector.magnitude) return;

        //forceVector = newForce;
        startForceVector = newForce;
        forceDuration = 0.1f * newForce.magnitude; 
        currentForceDuration = 0;
    }

    #endregion

    #region Easing

    private float EaseInExponential(float time, float start, float change, float duration)
    {
        return (time == 0) ? start : change * Mathf.Pow(2, 10 * (time / duration - 1)) + start;
    }

    private Vector3 EaseInExponential(float time, Vector3 start, Vector3 change, float duration)
    {
        return (time == 0) ? start : change * Mathf.Pow(2, 10 * (time / duration - 1)) + start;
    }

    private float EaseInQuart(float time, float start, float change, float duration)
    {
        return change * (time /= duration) * time * time * time + start;
    }

    private Vector3 EaseInQuart(float time, Vector3 start, Vector3 change, float duration)
    {
        return change * (time /= duration) * time * time * time + start;
    }

    #endregion

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + momentumVelocity);
        Gizmos.DrawWireSphere(sphereOrigin.position, groundedSphereRadius);
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (IsGrounded())
        {
            movementVelocity.y = 0;
            currentRescueTime = 0;
            bouncedOffHead = false;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //set the scene with the player charater as the active scene to avoid objects instantiating wrongly
        SceneManager.SetActiveScene(scene);
    }

    public void SetPaused(bool paused)
    {
        this.paused = paused;
    }
}
