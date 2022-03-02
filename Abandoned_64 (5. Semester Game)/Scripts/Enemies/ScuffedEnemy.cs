using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScuffedEnemy : Enemy
{
    [Header("Behaviour Variables")]
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private float attackIntervalTime = 3f;
    [SerializeField] private float maxDegreesTurnPerSecond = 90f;
    [SerializeField] private LayerMask obstructionLayers = -1;

    [Header("ScuffedEnemy Setup Variables")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource EnemySounds;
   // [SerializeField] private AudioClip passive;
    [SerializeField] private AudioSource PassiveEnemySounds;

    private Transform playerTarget;
    private float attackIntervalTimeCurrent = 3;
    [HideInInspector] public bool stunned;

    [SerializeField] private bool regenerateHealth = false;
    [SerializeField] private float healthRegenerationPerSecond = 5f;
    [SerializeField] private bool IsDead = false;
    //private PlayerStateMachine player;

    private new void Start()
    {
        base.Start();
        //EnemySounds.Play();
        //PassiveEnemySounds.Play();
        if (!PlayerStateMachine.Instance)
        {
            this.enabled = false;
            return;
        }

        playerTarget = PlayerStateMachine.Instance.transform;
        attackIntervalTimeCurrent = attackIntervalTime;
    }

    private void Update()
    {
        if (DamageLightup>=0)
        {
            DamageLightup -= Time.deltaTime*3;
            if (meshRenderer != null)
            {
                meshRenderer.material.SetFloat("Vector1_3c20468fd45f42bc80adaa488792a2e8", DamageLightup);
            }
        }

        //rotate enemy by 90rad a sec towards player if player is in range
        if (WithinRange() && NotBlocked()&&!IsDead)
        {
            anim.SetBool("IsIdle", true);
            //todo move player origin
            Vector3 targetDir = playerTarget.position - transform.position;
            Vector3 projectileDir = playerTarget.position - projectileSpawn.position;
            Vector3 planarToTarget = Vector3.ProjectOnPlane(targetDir, Vector3.up);
            Vector3 planarRoatation = Vector3.RotateTowards(transform.forward, planarToTarget, Mathf.Deg2Rad * maxDegreesTurnPerSecond * Time.deltaTime, 0f);

            if (!stunned)
            {
                transform.rotation = Quaternion.LookRotation(planarRoatation, Vector3.up);
                projectileSpawn.rotation = Quaternion.LookRotation(targetDir, Vector3.up);
                if (attackIntervalTime == attackIntervalTimeCurrent)
                {
                    if (!(currentHealth <= 0))
                        anim.SetBool("IsAttacking", true);
                }
            }
        }

        if(attackIntervalTimeCurrent != attackIntervalTime)
        {       
            attackIntervalTimeCurrent = Mathf.Clamp(attackIntervalTimeCurrent + Time.deltaTime, 0, attackIntervalTime);
        }

        if (regenerateHealth && currentHealth < startHealth)
            Regenerate(healthRegenerationPerSecond);
    }

    public override void TakeDamage(float damage)
    {

        base.TakeDamage(damage);
        stunned = true;
        if (!IsDead)
        {
            anim.SetTrigger("BeingHit" + Random.Range(1, 3));
        }
    }
    protected override void OnEnemyDeath()
    {
        base.OnEnemyDeath();
    }
    protected override void Die()
    {
        targetable = false;
        base.OnEnemyDeath();
        IsDead = true;
       // anim.SetBool("IsDead", true);
        anim.Play("Armature|Death");
        StartCoroutine(DeathDelay());

         
    }
    bool WithinRange()
    {
        return Vector3.Distance(playerTarget.position, transform.position) <= attackRange;
    }

    public override void Attack()
    {
        base.Attack();
        ShootProjectile();
    }


    public void ShootProjectile()
    {
        Instantiate(projectile, projectileSpawn.position, Quaternion.LookRotation(playerTarget.position + Vector3.up - projectileSpawn.position));
        attackIntervalTimeCurrent = 0;
    }
    private bool NotBlocked()
    {
        Vector3 origin = projectileSpawn.position;
        Vector3 direction = playerTarget.position + Vector3.up - origin;

        float radius = 0.15f;
        float distance = direction.magnitude;
        bool notBlocked = !Physics.SphereCast(origin, radius, direction, out RaycastHit hit, distance, obstructionLayers);

        return notBlocked;
    }
    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(10);
        base.Die();
    }
}
