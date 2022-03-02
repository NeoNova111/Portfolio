using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public static class InputActionExtensions
{
    public static bool IsPressed(this InputAction inputAction)
    {
        return inputAction.ReadValue<float>() > 0f;
    }

    public static bool WasPressedThisFrame(this InputAction inputAction)
    {
        return inputAction.triggered && inputAction.ReadValue<float>() > 0f;
    }

    public static bool WasReleasedThisFrame(this InputAction inputAction)
    {
        return inputAction.triggered && inputAction.ReadValue<float>() == 0f;
    }
}

public class PlayerStateMachine : MonoBehaviour, IDamagable
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Hitbox swordHitbox;
    public Hitbox SwordHitbox { get => swordHitbox; }

    private CameraController cameraController;
    private UIManager userInterfaceManager;
    private SaveManager saveManager;
    private InputDeviceManager deviceManager;
    public CameraController CameraController { get => cameraController; }
    public UIManager UIManager { get => userInterfaceManager; }
    public SaveManager SaveManager { get => saveManager; }

    //ScriptableObject Events
    [SerializeField] private GameEvent playerDied;
    [SerializeField] private GameEvent playerTookDamage;
    [SerializeField] private GameEvent playerLanded;

    public GameEvent PlayerLanded { get => playerLanded; }

    public GameEvent PlayerDied { get => playerDied; }
    public GameEvent PlayerTookDamage { get => playerTookDamage; }

    //stuff? taking better proposals
    private Rigidbody rb;
    private Collider playerCollider;
    private float colliderDistanceToGround;
    //todo: don't like this solution (preventing player to attack while airborne)
    private bool airborne = false;

    public PlayerStats PlayerStats { get => playerStats; }
    public Rigidbody Rb { get => rb; }
    public bool Airborne { get => airborne; set => airborne = value; }

    //state variables
    private bool invincible = false;
    public bool Invincible { get => invincible; set => invincible = value; }

    //interacting variables
    [SerializeField] private float interactRange = 5f;
    [SerializeField] private float talkingDelay = 1f;
    private float currentTalkingDelay;
    private IInteractable interactingWith;

    public float InteractRange { get => interactRange; }
    public bool CanTalk { get => currentTalkingDelay == 0; }
    public IInteractable InteractingWith { get => interactingWith;  set => interactingWith = value; }
    public bool Interacing { get => interactingWith != null && interactingWith.Interacting; }

    [Header("Movement Variabbles")]
    [SerializeField] private float movespeed = 5f;
    [SerializeField] private float runMultiplier = 1.5f;
    [SerializeField] private float jumpVelocity = 8f;
    [SerializeField] private LayerMask jumpableLayers;
    [SerializeField] private float minOverflowThreshold = -255f;
    [SerializeField] private float maxOverflowThreshold = 255f;
    [SerializeField] private float turnSmoothTime = 0.15f;
    private float turnSmoothVelocity;
    private Vector3 moveDir;
    [SerializeField] private float rollDistance = 2f;
    [SerializeField] private float rollMultiplier = 2f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float currentJumpBufferTime;
    [SerializeField] private float movementAccelTime = 0.2f;
    [SerializeField] private float movementDeccelTime = 0.3f;
    private float accelerationProgress;
    private bool sprinting = false;
    public bool Sprinting { get => sprinting; set => sprinting = value; }

    //movement getter setter
    public float Movespeed { get => movespeed; }
    public float RunMultiplier { get => runMultiplier; }
    public float RollMultiplier { get => rollMultiplier; }
    public float JumpVelocity { get => jumpVelocity; }
    public float TurnSmoothTime { get => turnSmoothTime; }
    public float RollDistance { get => rollDistance; }
    public float CoyoteTime { get => coyoteTime; }
    public float MovementAccelTime { get => movementAccelTime; }
    public float MovementDeccelTime { get => movementDeccelTime; }
    public float AccelerationProgress { get => accelerationProgress; set => accelerationProgress = value; }

    [Header("Animation Variables")]
    [SerializeField] private Animator animator;
    private bool isStanding;
    private bool isJumping;
    private bool infinteSwordGlitching = false;
    public Blinking blink;
    [SerializeField] private SkinnedMeshRenderer meshRender;
    [SerializeField] private float DamageColorStrength = 0;
    [SerializeField] public GameObject ISGEffect;
    //animation getter setter
    public Animator Animator { get => animator; }
    public bool IsJumping { get => IsJumping; set => isJumping = value; }
    public bool InfiniteSwordGlitching { get => infinteSwordGlitching; set => infinteSwordGlitching = value; }

    [Header("Gravitation Variables")]
    private float fallspeed;
    [SerializeField] private float groundedSphereRadius = 0.2f;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float maxFallVelocity = 30f;

    //grav getter setter
    public float FallSpeed { get => fallspeed; set => fallspeed = value; }
    public float FallMultiplier { get => fallMultiplier; set => fallMultiplier = value; }
    public float MaxFallVelocity { get => maxFallVelocity; }

    //input variables
    private PlayerInput input;
    private Vector2 currentMovement;
    private bool movementPressed;
    private bool runPressed;
    private bool jumpPressed;
    private bool lockOnPressed;
    private bool attackPressed;
    private bool leftPressed;
    private bool rightPressed;
    private bool debugPressed;
    private bool menuPressed;
    private bool returnPressed;
    private bool inContext;
    private bool contextPressed;

    public Vector2 CurrentMovement { get => currentMovement; }
    public bool IsRunPressed { get => runPressed; }
    public bool WasRunPressedThisFrame { get => input.CharacterControlls.Run.WasPressedThisFrame(); }
    public bool IsMovementPressed { get => movementPressed; }
    public bool IsJumpPressed { get => jumpPressed; }
    public bool WasJumpPressedThisFrame { get => input.CharacterControlls.Jump.WasPressedThisFrame(); }
    public bool WithinJumpBufferTime { get { return currentJumpBufferTime < jumpBufferTime; } }
    public bool IsLockOnPressed { get => lockOnPressed; }
    public bool WasLockOnThisFrame { get => input.CharacterControlls.LockOn.WasPressedThisFrame(); }
    public bool IsAttackPressed { get => attackPressed; }
    public bool WasAttackPressedThisFrame { get => input.CharacterControlls.Attack.WasPressedThisFrame(); }
    public bool IsLeftPressed { get => leftPressed; }
    public bool WasLeftPressedThisframe { get => input.CharacterControlls.TargetLeft.WasPressedThisFrame(); }
    public bool IsRightPressd { get => rightPressed; }
    public bool WasRightPressdThisFrame { get => input.CharacterControlls.TargetRight.WasPressedThisFrame(); }
    public bool IsDebugPressed { get => debugPressed; }
    public bool WasDebugPressedThisFrame { get => input.CharacterControlls.Debugmode.WasPressedThisFrame(); }
    public bool IsMenuPressed { get => menuPressed; }
    public bool WasMenuPressedThisFrame { get => input.CharacterControlls.Menu.WasPressedThisFrame(); }
    public bool IsReturnPressed { get => menuPressed; }
    public bool WasReturnPressedThisFrame { get => input.CharacterControlls.Return.WasPressedThisFrame(); }
    public bool InContext { get => inContext; set => inContext = value; }
    public bool WasContextPressedThisFrame { get => input.CharacterControlls.ContextButton.WasPressedThisFrame(); }
    public bool IsContextPressed { get => contextPressed; }

    private bool locked = false;
    public bool Locked { get => locked; }

    private bool doAccidentialAttack = false;
    public bool DoAccidentialAttack { get => doAccidentialAttack; set => doAccidentialAttack = value; }
    private bool forceWalk = false;
    public bool ForceWalk { get => forceWalk; set => forceWalk = value; }
    private bool gotHit = false;
    public bool GotHit { get => gotHit; set => gotHit = value; }
    private bool healing = false;
    public bool Healing { get => healing; set => healing = value; }

    [SerializeField] private bool damagable = true;
    public bool Damagable { get => damagable; set => damagable = value; }

    private PlayerBaseState rootState;
    private PlayerStateFactory states;
    private bool rootChanged = false;
    
    public PlayerBaseState RootState { get => rootState; set => rootState = value; }
    public bool RootChanged { get => rootChanged; set => rootChanged = value; }

    //singleton
    private static PlayerStateMachine instance;
    public static PlayerStateMachine Instance { get => instance; }

    private void Awake()
    {
        //register input callbacks
        input = new PlayerInput();
        input.CharacterControlls.Movement.performed += OnMovementInput;
        input.CharacterControlls.Movement.canceled += OnMovementInputCanceled;
        input.CharacterControlls.Run.performed += OnRun;
        input.CharacterControlls.Jump.performed += OnJumpPressed;
        input.CharacterControlls.LockOn.started += OnLockOnPressed;
        input.CharacterControlls.Attack.performed += ctx => attackPressed = ctx.ReadValueAsButton();
        input.CharacterControlls.ContextButton.performed += ctx => contextPressed = ctx.ReadValueAsButton();
        input.CharacterControlls.TargetLeft.performed += ctx => leftPressed = ctx.ReadValueAsButton();
        input.CharacterControlls.TargetRight.performed += ctx => rightPressed = ctx.ReadValueAsButton();
        input.CharacterControlls.Debugmode.performed += OnDebugPressed;
        input.CharacterControlls.LookAround.performed += OnLookingAround;

        //set vars
        if(!animator)
            animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();

        //setup statemachine
        states = new PlayerStateFactory(this);
        rootState = states.Alive();
        rootState.EnterState();

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        //set singleton references
        deviceManager = InputDeviceManager.Instance;
        userInterfaceManager = UIManager.Instance;
        cameraController = CameraController.Instance;
        saveManager = SaveManager.Instance;

        //deserialization stuff
        if (saveManager.PlayerLoadData != null)
        {
            InterpretLoadData();
        }
        else
        {
            playerStats.allowedToUseDebug = true;
            playerStats.CurrentHealth = playerStats.maxHealth;
            playerStats.collectibleCount = 0;
            playerStats.keyCount = 0;
            playerStats.StatsChanged.Raise();
        }

        playerStats.PlayerTransform = transform;
        saveManager.SavePlayerData();


        Cursor.lockState = CursorLockMode.Locked;
        colliderDistanceToGround = Vector3.Distance(playerCollider.bounds.min, playerCollider.bounds.center);
        currentJumpBufferTime = jumpBufferTime;
        currentTalkingDelay = 0;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        AnyStateUpdate();
        rootState.UpdateStates();
        PullCollectables(3.5f);
        if (DamageColorStrength >= 0)
        {
            DamageColorStrength -= Time.deltaTime;
            for (int i = 0; i < 8; i++)
            {
                meshRender.materials[i].SetFloat("Vector1_fad0febd136f4f4591520c731c6f9a46", DamageColorStrength);
            }
        }
        if (DamageColorStrength < 0)
        {
            DamageColorStrength = 0;
        }
    }

    void ToggleDebug()
    {
        Debug.Log(rootState.PrintHirarchy(rootState));
        DebugModeManager.Instance.ToggleDebugMode();
    }

    public void ToggleSprint(bool active)
    {
        if (active == sprinting)
            return;

        sprinting = active;
    }

    public void TakeDamage(float damage)
    {
        if (damage == 0 || invincible)
            return;
        DamageColorStrength = 1;
        playerStats.CurrentHealth = Mathf.RoundToInt(Mathf.Clamp(playerStats.CurrentHealth - damage, 0, playerStats.maxHealth));
        playerTookDamage.Raise();
        gotHit = true; //this var is used to trigger a substate change on the same level as walking state, since there is currently no other way to directly adress substates from the statemachine directly
        
    }

    public void Respawn()
    {
        //todo: only works if the "main scene" is first
        saveManager.LoadPlayerData();

        for(int i  = SceneManager.sceneCount - 1; i > 0; i--)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
        }

        transform.position = saveManager.PlayerLoadData.savePosition;
        transform.rotation = saveManager.PlayerLoadData.saveRotation;
        playerStats.collectibleCount = saveManager.PlayerLoadData.collectibleCount;
        playerStats.keyCount = saveManager.PlayerLoadData.keyItemCount;
        playerStats.CurrentHealth = playerStats.maxHealth;
        playerStats.StatsChanged.Raise();
    }

    public void ForceInteract(IInteractable interactable)
    {
        rootState.ForceSwitch(states.Interact(interactable));
    }

    private void InterpretLoadData()
    {
        saveManager.LoadPlayerData();

        #if UNITY_STANDALONE && !UNITY_EDITOR
        //might cause problems later on when adding/ changing Scene index in build settings, scene name is also not foolproof though... maybe a combination?
        if(SceneManager.GetActiveScene().buildIndex == saveManager.PlayerLoadData.levelIdx)
        {
            transform.position = saveManager.PlayerLoadData.savePosition;
            transform.rotation = saveManager.PlayerLoadData.saveRotation;
        }
        #endif

        playerStats.collectibleCount = saveManager.PlayerLoadData.collectibleCount;
        playerStats.keyCount = saveManager.PlayerLoadData.keyItemCount;
        playerStats.maxHealth = saveManager.PlayerLoadData.playerMaxHealth;
        playerStats.CurrentHealth = saveManager.PlayerLoadData.playerCurrentHealth;
        playerStats.allowedToUseDebug = saveManager.PlayerLoadData.allowedToUseDebug;
        playerStats.StatsChanged.Raise();
    }

    //similar to Alive state, but not quite
    void AnyStateUpdate()
    {
        if (WasRunPressedThisFrame)
        {
            ToggleSprint(!sprinting);
        }

        if (transform.position.y <= minOverflowThreshold)
        {
            transform.position = new Vector3(transform.position.x, maxOverflowThreshold, transform.position.z);
        }

        if (WasDebugPressedThisFrame && playerStats.allowedToUseDebug)
        {
            ToggleDebug();
        }

        //maybe get rid of weird sliding?
        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        if (WasJumpPressedThisFrame)
        {
            currentJumpBufferTime = 0;
        }
        else if (currentJumpBufferTime < jumpBufferTime)
        {
            currentJumpBufferTime = Mathf.Clamp(currentJumpBufferTime + Time.deltaTime, 0, jumpBufferTime);
        }

        if(currentTalkingDelay > 0)
        {
            currentTalkingDelay = Mathf.Clamp(currentTalkingDelay - Time.deltaTime, 0, talkingDelay);
        }
    }

    public bool IsRunning()
    {
        switch (deviceManager.ActiveDeviceType)
        {
            case DeviceType.Controller:
                return sprinting;
            case DeviceType.MK:
                return IsRunPressed;
            default:
                return false;
        }
    }

    public bool IsGrounded()
    {
        bool grounded = Physics.SphereCast(playerCollider.bounds.center, groundedSphereRadius, Vector3.down, out RaycastHit hit, colliderDistanceToGround, jumpableLayers);
        return grounded;
    }

    public void StartTalkingDelay()
    {
        currentTalkingDelay = talkingDelay;
    }

    public IInteractable GetClosestInteractable(float distance)
    {
        if (Interacing)
        {
            return interactingWith;
        }

        List<IInteractable> interactables = new List<IInteractable>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance);

        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.Interactable)
            {
                interactables.Add(interactable);
            }
        }

        if (interactables.Count <= 0)
            return null;

        float currentDistance;
        float smallestDistance = Mathf.Infinity;
        IInteractable closestToPlayer = null;

        foreach (IInteractable interactable in interactables)
        {
            currentDistance = Vector3.Distance(interactable.TargetTransform.position, transform.position);
            if (smallestDistance > currentDistance)
            {
                closestToPlayer = interactable;
                smallestDistance = currentDistance;
            }
        }

        interactingWith = closestToPlayer;
        return closestToPlayer;
    }

    public void PullCollectables(float distance)
    {
        List<IPullable> pullables = new List<IPullable>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance);

        foreach (Collider collider in colliders)
        {
            IPullable pullable = collider.GetComponent<IPullable>();
            if (pullable != null && pullable.Pullable)
            {
                pullables.Add(pullable);
            }
        }

        foreach(IPullable p in pullables)
        {
            p.Pull();
        }
    }

    #region Input Callbacks

    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovement = context.ReadValue<Vector2>();
        movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
    }

    void OnMovementInputCanceled(InputAction.CallbackContext context)
    {
        currentMovement = context.ReadValue<Vector2>();
        movementPressed = false;
    }

    void OnLookingAround(InputAction.CallbackContext context)
    {
        //for debugging purpouses
    }

    void OnRun(InputAction.CallbackContext contex)
    {
        runPressed = contex.ReadValueAsButton();
    }

    void OnJumpPressed(InputAction.CallbackContext contex)
    {
        jumpPressed = contex.ReadValueAsButton();
    }

    void OnLockOnPressed(InputAction.CallbackContext context)
    {
        lockOnPressed = context.ReadValueAsButton();
    }

    void OnDebugPressed(InputAction.CallbackContext context)
    {
        debugPressed = context.ReadValueAsButton();
    }

#endregion

    private void OnEnable()
    {
        input.CharacterControlls.Enable();
    }

    private void OnDisable()
    {
        input.CharacterControlls.Disable();
    }
}
