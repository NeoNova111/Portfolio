using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaBossStateMachine : EnemyHierarchicalStateMachine, IDamagable
{
    public GameObject Fireball { get => fireBall; }

    [SerializeField] private GameObject stompCollider;
    [SerializeField] private GameObject impact;
    [SerializeField] private Transform impactParent;
    [SerializeField] private GameObject shockWave;
    [SerializeField] private GameObject healthBarObject;

    [SerializeField] private GameObject leftFoot;
    [SerializeField] private GameObject righztFoot;
    [SerializeField] private GameObject fireBall;

    public GameEvent ActivateBoss { get => activated; }
    [SerializeField] private GameEvent activated;
    [SerializeField] private GameEvent defeated;

    [Header("Transition")]
    public float jumpDuration = 2f;
    public AnimationCurve jump;
    [HideInInspector] public bool jumping = false;
    public float meteorStartDistance = 100f;
    public float meteorDuration = 4f;
    [Range(0, 1f)] public float meteorTracking = 0.6f;
    public float meteorDelay = 1f;
    public AnimationCurve meteorSpeed;
    public AnimationCurve meteorTrajectory;

    [Header("Phase2")]
    [Range(0, 1f)] public float phaseTransitionThreshold = 0.4f;
    [Range(0, 1f)] public float desperationThreshold = 0.1f;
    public float speedMultiplier;
    public bool transitioned = false;
    [HideInInspector]public bool transitioning = false;
    private bool desperationAttackUsed = false;
    
    private new void Start()
    {
        base.Start();
        fireBall.SetActive(false);
    }

    private new void Update()
    {
        base.Update();
        if(currentHealth / maxHealth <= phaseTransitionThreshold && !transitioned && !transitioning)
        {
            TransitionPhase();
        }
        else if(currentHealth / maxHealth <= desperationThreshold && !desperationAttackUsed)
        {
            _currentState.SwitchState(_states.PandaBossAttacks(true));
            desperationAttackUsed = true;
        }
    }

    public void Impact()
    {
        Instantiate(impact, impactParent.position, Quaternion.identity);
        AkSoundEngine.PostEvent("Ground_Shake", PlayerController.Instance.gameObject);
    }

    //wrapping for animation events
    public void ActivateStomp()
    {
        stompCollider.SetActive(true);
    }

    //wrapping for animation events
    public void DeactivateStomp()
    {
        stompCollider.SetActive(false);
    }

    public void SendShockWave(int foot)
    {
        AkSoundEngine.PostEvent("Ground_Shake", PlayerController.Instance.gameObject);
        if (foot == 0) Instantiate(shockWave, leftFoot.transform.position, Quaternion.identity);
        else Instantiate(shockWave, righztFoot.transform.position, Quaternion.identity);
    }

    public void TransitionPhase()
    {
        _currentState.SwitchState(_states.PandaBossTransition(this));
    }

    public void StartJump()
    {
        AkSoundEngine.PostEvent("Ground_Shake", PlayerController.Instance.gameObject);
        AkSoundEngine.PostEvent("Enemy_Miss", gameObject);
        jumping = true;
    }

    public void HeavyStep()
    {
        AkSoundEngine.PostEvent("Enemy_Steps", gameObject);
    }

    public new void TakeDamage(float damage)
    {
        if (currentHealth / maxHealth <= desperationThreshold && !desperationAttackUsed) return; //ensure that the desperation attack gets used even if the boss takes damage multiple times during one frame surpassing the threshold
        else if (currentHealth <= damage && !desperationAttackUsed) damage = currentHealth - desperationThreshold * maxHealth;

        base.TakeDamage(damage);
        if(currentHealth <= 0) defeated.Raise();
        UIManager.Instance.BossHP.SetHealth(currentHealth / maxHealth);
    }

    //wrappers for animation events
    public void GoInvincible()
    {
        Invincible(true);
    }

    public void LeaveInvincible()
    {
        Invincible(false);
    }

    public void Invincible(bool isTrue)
    {
        if (isTrue)
        {
            damagable = false;
        }
        else
        {
            damagable = true;
        }
        UIManager.Instance.BossHP.ChangeDamagable(damagable);
    }
}
