using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Direction { FORWARD, BACKWARD };

public class submarineMovement : MonoBehaviour
{
    #region Singleton

    public static submarineMovement instance;

    public void Awake()
    {
        if(instance != null)
        {
            return;
        }
        instance = this;
    }

    #endregion

    #region vars

    public SubStats submarineStats;
    public Crewmember captain; //only one crewmember

    //movement stuff
    Vector3 turnForce;
    Vector3 forwardForce;
    Vector3 upwardForce;

    bool rotating = true;
    Direction dir = Direction.FORWARD;

    CameraControl cameraControl;

    Rigidbody subRB;

    public GameEvent CollisionEvent;
    float collisionInvulnDelay = 0;

    public GameEvent saveEvent;

    bool dead = false;
    bool diving;

    //vars for sideview adjustments
    bool adjusting;
    float adjustingDelta;
    bool enteredRightway;

    Room currentRoom;

    //Respawn stuff
    public Vector3 respawnPos;
    public Quaternion respawnRot;

    public Light AlertLight;
    public GameObject lookAtPrefab;

    #endregion

    #region Setup
    private void OnEnable()
    {
        Instantiate(lookAtPrefab);
        UpdateSubStats();
    }

    void Start()
    {
        if (SaveManager.instance.loadFromSave)
        {
            transform.position = SaveManager.instance.loadData.savePosition;
        }

        SavePoint();

        subRB = GetComponent<Rigidbody>();
        adjusting = false;
        diving = false;

        ZeroOutMovement();

        //camera stuff maybe move when cleaning up code
        cameraControl = CameraControl.instance;
        cameraControl.SetTarget(gameObject.transform);

        respawnPos = transform.position;
        respawnRot = transform.rotation;

    }

    public void SideViewSetup(float rotTime)
    {
        adjusting = true;

        float angle = Vector3.Angle(transform.forward, currentRoom.transform.forward);
        adjustingDelta = angle / rotTime;

        Debug.Log("angle: "+angle+", delta: "+adjustingDelta);

        if (Input.GetAxis("Vertical") >= 0) //differentiate between entering forward or backwards
        {
            if (angle <= 90)
                enteredRightway = true;
            else
                enteredRightway = false;
        }
        else
        {
            if (angle <= 90)
                enteredRightway = false;
            else
                enteredRightway = true;
        }
    }

    #endregion

    #region Updates

    private void Update() //not caching "stats" in start to allow for runtime gameplay adjustments using the scriptable object
    {
        //maybe move out of update and change when it changes, but this is easier for now
        AlertLight.intensity = (1 - (submarineStats.health / submarineStats.maxHealth)) * submarineStats.maxDamageLightIntensity;
        AlertLight.range = (1 - (submarineStats.health / submarineStats.maxHealth)) * submarineStats.maxDamageLightRange;

        if (submarineStats.respawned)
        {
            submarineStats.health = submarineStats.maxHealth;
            submarineStats.respawned = false;
        }

        if (collisionInvulnDelay > 0)
            collisionInvulnDelay -= Time.deltaTime;

        if (dead)
        {
            subRB.angularDrag = submarineStats.angulatDrag * 0.2f;
            subRB.drag = submarineStats.drag * 0.2f;
            return;
        }

        subRB.drag = submarineStats.drag;
        subRB.angularDrag = submarineStats.angulatDrag;

        switch (currentRoom.perspective)
        {
            case CameraPerspective.TOPDOWN:
                CalculateTopdownMovementInput();
                break;
            case CameraPerspective.POV:
                CalculateTopdownMovementInput();
                break;
            case CameraPerspective.SIDE:
                CalculateSideviewMovement();
                break;
        }

        if (diving)
            AutoDive();
    }

    void FixedUpdate()
    {
        if (dead)
        {
            if (Input.anyKeyDown)
            {
                Respawn();
            }
            return;
        }

        ApplyMovementForce();
    }

    private void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        UpdateSubStats();
    }

    #endregion

    #region Movement Calculations

    void CalculateTopdownMovementInput()
    {
        subRB.constraints = RigidbodyConstraints.None;

        if(!diving)
            subRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        else
            subRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        turnForce = new Vector3(0, Input.GetAxis("Horizontal"), 0) * submarineStats.turnMomentum * (1 - captain.currentFearIntensity);
        forwardForce = new Vector3(0, 0, Input.GetAxis("Vertical"));

        if (forwardForce.z > 0)
            forwardForce *= submarineStats.forwardMomentum;
        else if (forwardForce.z < 0)
            forwardForce *= submarineStats.backwardMomentum;

        if (Input.GetKey(KeyCode.Q))
        {
            forwardForce += Vector3.left * submarineStats.strafeMomentum;
        }

        if (Input.GetKey(KeyCode.E))
        {
            forwardForce += Vector3.right * submarineStats.strafeMomentum;
        }
    }

    void CalculateSideviewMovement()
    {
        subRB.constraints = RigidbodyConstraints.None;
        subRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if (adjusting)
        {
            if (enteredRightway)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(currentRoom.transform.eulerAngles), adjustingDelta * Time.deltaTime);
            else
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(currentRoom.transform.eulerAngles + new Vector3(0, 180, 0)), adjustingDelta * Time.deltaTime);

            if (transform.rotation == Quaternion.Euler(currentRoom.transform.eulerAngles) || transform.rotation == Quaternion.Euler(currentRoom.transform.eulerAngles + new Vector3(0, 180, 0)))
                adjusting = false;

            return;
        }

        upwardForce = Vector3.up * verticalInput * submarineStats.diveMomentum;
        RotSnap(horizontalInput);

        if (dir == Direction.FORWARD && !rotating)
        {
            if (horizontalInput >= 0)
                forwardForce = Vector3.forward * horizontalInput;
            else
            {
                SideViewRotStart();
            }
        }
        else if (dir == Direction.BACKWARD && !rotating)
        {
            if (horizontalInput <= 0)
                forwardForce = Vector3.forward * -horizontalInput;
            else
            {
                SideViewRotStart();
            }
        }
        else if(rotating)
        {
            turnForce = new Vector3(0, horizontalInput, 0) * submarineStats.sideturnMomentum;
        }

        forwardForce *= submarineStats.forwardMomentum;
    }

    void AutoDive()
    {
        upwardForce = Vector3.down * submarineStats.diveMomentum;
    }

    void ApplyMovementForce()
    {
        subRB.AddRelativeTorque(turnForce, ForceMode.Acceleration);
        subRB.AddRelativeForce(forwardForce, ForceMode.Acceleration);
        subRB.AddRelativeForce(upwardForce, ForceMode.Acceleration);
    }

    void RotSnap(float input)
    {
        if (Quaternion.Angle(transform.rotation, currentRoom.transform.rotation) <= 2 && rotating && dir == Direction.BACKWARD && input >= 0)
        {
            transform.forward = currentRoom.transform.forward;
            dir = Direction.FORWARD;
            subRB.angularVelocity = Vector3.zero;
            turnForce = Vector3.zero;
            rotating = false;
        }
        else if (Quaternion.Angle(transform.rotation, currentRoom.transform.rotation) >= 178 && rotating && dir == Direction.BACKWARD && input <= 0)
        {
            transform.forward = -currentRoom.transform.forward;
            subRB.angularVelocity = Vector3.zero;
            turnForce = Vector3.zero;
            rotating = false;
        }

        if (Quaternion.Angle(transform.rotation, currentRoom.transform.rotation) >= 178 && rotating && dir == Direction.FORWARD && input <= 0)
        {
            transform.forward = -currentRoom.transform.forward;
            dir = Direction.BACKWARD;
            subRB.angularVelocity = Vector3.zero;
            turnForce = Vector3.zero;
            rotating = false;
        }
        else if (Quaternion.Angle(transform.rotation, currentRoom.transform.rotation) <= 2 && rotating && dir == Direction.FORWARD && input >= 0)
        {
            transform.forward = currentRoom.transform.forward;
            subRB.angularVelocity = Vector3.zero;
            turnForce = Vector3.zero;
            rotating = false;
        }
    }

    void SideViewRotStart()
    {
        turnForce = new Vector3(0, Input.GetAxis("Horizontal"), 0) * submarineStats.turnMomentum;
        forwardForce = Vector3.zero;
        subRB.velocity = Vector3.zero;
        rotating = true;
    }

    #endregion

    #region Collision
    //The "AlertLight" Shenanigans are all done by Oliver, this probably isn't as cool and impeccable ("lol impeccable" -Carlo) as Carlos stuff so just so you know I did this lol.
    //I just want people to be able to tell the ship is damaged.

    void ObstacleCollision(Collision other)
    {
        
        DamagingObject obj = other.gameObject.GetComponent<DamagingObject>();

        if (obj)
        {
            dead = submarineStats.TakeDamage(obj.damage);
        }
        else
        {
            dead = submarineStats.TakeDamage(submarineStats.defaultCollisionDamage);
        }

        collisionInvulnDelay = submarineStats.collisionInvulnTime;
        CollisionEvent.Raise();

        int size = other.contacts.Length;

        foreach(ContactPoint contact in other.contacts)
        {
            subRB.AddForceAtPosition(contact.normal * ((submarineStats.collisionRepelForce * subRB.velocity.magnitude) / size), contact.point, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Invisible")
            return;

        if (collisionInvulnDelay <= 0)
        {
            ObstacleCollision(other);
        }
    }

    #endregion

    #region Utility

    public void SetCurrentRoom(Room room)
    {
        if(currentRoom != null && room.perspective != currentRoom.perspective)
        {
            ZeroOutMovement();
        }
        currentRoom = room;
    }

    void Teleport(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
        submarineStats.teleport.Raise();
    }

    void UpdateSubStats()
    {
        submarineStats.submarinePosition = transform.position;
        submarineStats.submarineEulerAngles = transform.eulerAngles;
        submarineStats.submarineRotation = transform.rotation;

        //submarineStats.subTransform = transform;
    }

    void Respawn()
    {
        //reset vars
        ZeroOutMovement();
        dead = false;
        submarineStats.respawned = true;
        AlertLight.intensity = 0;

        //teleport/ respawn
        TransitionManager tm = TransitionManager.instance;
        submarineStats.respawn.Raise();
        tm.HardCut(submarineStats.respawnTime, respawnPos);
        transform.rotation = respawnRot;
    }

    public void SavePoint()
    {
        saveEvent.Raise();
        respawnPos = transform.position;
        respawnRot = transform.rotation;
        SaveManager.instance.SavePlayerData();
    }

    public void ZeroOutMovement()
    {
        forwardForce = Vector3.zero;
        turnForce = Vector3.zero;
        upwardForce = Vector3.zero;

        subRB.velocity = Vector3.zero;
        subRB.angularVelocity = Vector3.zero;
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public void SetDiving(bool isDiving)
    {
        diving = isDiving;
    }

    public bool IsRotating()
    {
        return rotating;
    }

    public void SetDead(bool d)
    {
        dead = d;
    }
    #endregion
}
