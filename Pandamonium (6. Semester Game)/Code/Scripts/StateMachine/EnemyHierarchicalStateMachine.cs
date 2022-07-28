using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHierarchicalStateMachine : MonoBehaviour, IDamagable
{

    //contains all variables 
    //assign variables
    //base funktions

    //functions that are called in every class
    public NavMeshAgent navmeshagent { get { return _navmeshagent; } set { _navmeshagent = value; } }
    public Transform PlayerTransform { get { return _playerTransform; } set { _playerTransform = value; } }
    public Vector3 directions { get { return _directions; } set { _directions = value; } }
    public GameObject enemy { get { return _enemyObject; } set { _enemyObject = value; } }
    public float distanceToPlayer { get { return _distanceToPlayer; } set { _distanceToPlayer = value; } }
    public EnemyBaseState currentState { get { return _currentState; } set { _currentState = value; } }
    public Animator enemyanimation { get { return _enemyanimation; } set { _enemyanimation = value; } }
    public float seerange { get { return _seerange; } set { _seerange = value; } }
    public LayerMask ViewObstuctionLayers { get => _viewObstuctionLayers; }
    public bool Passive { get => passive; set => passive = value; }

    [SerializeField] protected bool damagable;
    public bool Damagable { get => damagable; set => damagable = value; }

    [SerializeField] protected Transform transformOverwrite;
    public Transform Transform { get => transformOverwrite ? transformOverwrite : transform; }

    //state machine stuff
    protected EnemyBaseState _currentState;
    protected EnemyStateFactory _states;
    protected Transform _playerTransform;
    protected PlayerController player;
    protected float _distanceToPlayer;
    [SerializeField] protected Animator _enemyanimation;

    [Header("Sounds")]
    public AK.Wwise.Event voice;
    public AK.Wwise.Event attackSound;
    public Vector2 voiceRndTimer = new Vector2(4f, 10f);
    private IEnumerator voiceRoutine;

    [Header("NavMeshStats")]
    [SerializeField] protected NavMeshAgent _navmeshagent;
    protected Vector3 _directions;
    protected GameObject _enemyObject;

    [Header("Enemy Vision")]
    [SerializeField] protected float _seerange = 40f;
    [SerializeField] protected LayerMask _viewObstuctionLayers;

    //enemystats
    [Header("EnemyStats")]
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;

    [Header("EnemyHitReaction")]
    [SerializeField] protected float hitStopTime = 0.05f;
    [SerializeField] protected float knockbackForce = 2f;
    protected IEnumerator hitstopRoutine;
    protected bool hitstopRunning = false;

    [SerializeField] protected Transform tinyGuySpawn;
    public Transform TinyGuySpawn { get => tinyGuySpawn ? tinyGuySpawn : transform; }
    [SerializeField] protected GameObject tinyGuy;

    protected Transform lookAtTarget;

    [Header("Enemy Attacks")]
    [SerializeField] protected float attackRange = 2f;
    public float Attackrange { get => attackRange; }
    [SerializeField] protected GameObject attackCollider;
    public GameObject AttackCollider { get => attackCollider; }

    //visuals
    [SerializeField] protected ParticleSystem dustCloud;
    public ParticleSystem DustCloud { get => dustCloud; set => dustCloud = value; }

    [HideInInspector] public int skippedShockwaves = 0;

    protected bool passive = false;

    protected Rigidbody rb;

    protected void Awake()
    {
        _enemyObject = this.gameObject;
        dustCloud.Stop();

        _states = new EnemyStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();

        rb = GetComponent<Rigidbody>();
    }

    protected void Start()
    {
        if (PlayerController.Instance)
        {
            player = PlayerController.Instance;
            _playerTransform = PlayerController.Instance.transform;
            lookAtTarget = _playerTransform;
        }

        _distanceToPlayer = Mathf.Infinity;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!_playerTransform && PlayerController.Instance)
        {
            player = PlayerController.Instance;
            _playerTransform = PlayerController.Instance.transform;
            lookAtTarget = _playerTransform;
        }

        _currentState.UpdateState();

        if(lookAtTarget)
            enemy.transform.LookAt(new Vector3(lookAtTarget.position.x, transform.position.y, lookAtTarget.position.z), Vector3.up);
    }

    void OnEnable()
    {
        //Random animal sounds 
        voiceRoutine = VoiceCoroutine();
        StartCoroutine(voiceRoutine);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void TakeDamage(float damage)
    {
        if (passive) return; //we don't want to hurt any passive enemies, now do we?

        AkSoundEngine.PostEvent("Enemy_Hit", gameObject); //hitmarker sound
        if (UIManager.Instance) UIManager.Instance.HitMarker.AddCurrentDamage(damage);

        if (hitstopRunning)
        {
            StopCoroutine(hitstopRoutine);
            hitstopRunning = false;
        }
        hitstopRoutine = HitStopRoutine();
        StartCoroutine(hitstopRoutine);

        rb.AddRelativeForce(Vector3.back * knockbackForce, ForceMode.Impulse);

        if (currentHealth > 0)
        {
            currentHealth -= damage;
            if (currentHealth <= 0) Die();
        }
    }

    //these get called in animation events to 
    public void ActivateAttack()
    {
        attackCollider.SetActive(true);
        attackSound.Post(gameObject);
    }

    public void DeactivateAttack()
    {
        attackCollider.SetActive(false);
    }

    protected void Die()
    {
        Instantiate(tinyGuy, TinyGuySpawn.position, Quaternion.identity);
        Destroy(gameObject);
    }

    protected IEnumerator VoiceCoroutine()
    {
        while (true)
        {
            float rnd = Random.Range(voiceRndTimer.x, voiceRndTimer.y);
            yield return new WaitForSeconds(rnd);
            voice.Post(gameObject);
        }
    }

    protected IEnumerator HitStopRoutine()
    {
        //rb velocity set to zero frames this, because force is applied inbetween, and this coroutine can interrupt itself
        rb.velocity = Vector3.zero;
        hitstopRunning = true;
        navmeshagent.isStopped = true;
        rb.isKinematic = false;
        rb.useGravity = true;

        yield return new WaitForSeconds(hitStopTime);
        navmeshagent.isStopped = false;
        hitstopRunning = false;
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }
}