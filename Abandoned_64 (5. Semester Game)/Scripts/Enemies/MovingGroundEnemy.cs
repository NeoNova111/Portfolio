using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGroundEnemy : Enemy
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float detectRange = 15f;
    [SerializeField] private float attackIntervalTime = 3f;
    [SerializeField] private float moveIntervalTime = 3f;
    [SerializeField] private float moveDuration = 3f;
    [SerializeField] private float anticipationRotationSpeed = 1f;
    [SerializeField] private float healthRegenerationPerSecond = 15f;

    [SerializeField] protected GameEvent attackHit;
    [SerializeField] protected GameEvent digStart;
    [SerializeField] protected GameEvent digEnd;

    [SerializeField] private Transform areaCenter;
    [SerializeField] private float areaRadius;

    //[SerializeField] private GameObject parasiteObject;

    [SerializeField] private Animator animator;

    private bool underground = false;
    public bool Underground { get => underground; set => underground = value; }

    private float attackIntervalTimeCurrent = 3;
    private float MoveTimeCurrent = 3;

    //private bool attackInitiated = false;

    public event Action<MovingGroundEnemy> OnDeath;

    private Transform playerTarget = null;

    private MovingEnemyState state;
    public MovingEnemyState State { get => state; set => state = value; }

    public enum MovingEnemyState { Idle, Attacking, Anticipation, Moving }

    private new void Start()
    {
        base.Start();
        playerTarget = PlayerStateMachine.Instance.transform;
        attackIntervalTimeCurrent = attackIntervalTime;
        MoveTimeCurrent = 0;
        state = MovingEnemyState.Idle;
    }

    bool WithinDetectRange()
    {
        return Vector3.Distance(playerTarget.position, transform.position) <= detectRange;
    }

    bool WithinAttackRange()
    {
        return Vector3.Distance(playerTarget.position, transform.position) <= attackRange;
    }

    private void Update()
    {
        //Debug.Log(state);
        if (WithinDetectRange())
        {

            Vector3 targetDir = playerTarget.position - transform.position;
            Vector3 planarToTarget = Vector3.ProjectOnPlane(targetDir, Vector3.up);

            switch (state)
            {
                case MovingEnemyState.Idle:
                    //todo move player origin

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(planarToTarget, Vector3.up), 1000 * Time.deltaTime);

                    if (WithinAttackRange())
                    {
                        if (attackIntervalTime == attackIntervalTimeCurrent)
                        {
                            if (currentHealth > 0)
                            {
                                Attack();
                            }
                        }
                    }
                    else// if (!attackInitiated)
                    {
                        if (moveIntervalTime == MoveTimeCurrent)
                        {
                            if (currentHealth > 0)
                            {
                                Move();
                            }
                        }

                        if (MoveTimeCurrent != moveIntervalTime)
                        {
                            MoveTimeCurrent = Mathf.Clamp(MoveTimeCurrent + Time.deltaTime, 0, moveIntervalTime);
                        }
                    }

                    if (attackIntervalTimeCurrent != attackIntervalTime)
                    {
                        attackIntervalTimeCurrent = Mathf.Clamp(attackIntervalTimeCurrent + Time.deltaTime, 0, attackIntervalTime);
                    }
                    break;
                case MovingEnemyState.Moving:

                    if (moveDuration == MoveTimeCurrent)
                    {
                        //if (currentHealth > 0)
                        //{
                        /*
                            Vector2 randomPos = UnityEngine.Random.insideUnitCircle * attackRange;

                            Ray ray = new Ray(playerTarget.transform.position + (transform.up * 2), (playerTarget.transform.up * -1));

                            Debug.Log("Casting Ray!");
                            Debug.DrawRay(playerTarget.transform.position + (transform.up * 2), (playerTarget.transform.up * -1));
                            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                            {
                                Emerge(new Vector3(randomPos.x, hit.point.y, randomPos.y));
                                Debug.LogWarning(hit.collider.gameObject.name);
                            }
                            */
                        GetMovePosition();
                            
                        //}
                    }

                    if (MoveTimeCurrent != moveDuration)
                    {
                        MoveTimeCurrent = Mathf.Clamp(MoveTimeCurrent + Time.deltaTime, 0, moveDuration);
                    }
                    break;
                case MovingEnemyState.Anticipation:

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(planarToTarget, Vector3.up), anticipationRotationSpeed * Time.deltaTime);

                    break;
            }
            
        }

        if(currentHealth < startHealth)
            Regenerate(healthRegenerationPerSecond);
    }

    private void GetMovePosition()
    {
        if (Vector3.Distance(areaCenter.position, playerTarget.transform.position) > areaRadius)
        {
            return;
        }

        Vector3 pos = GetPosInArea();

        Emerge(pos);
    }

    private Vector3 GetPosInArea()
    {
        Vector2 randomPos = UnityEngine.Random.insideUnitCircle * attackRange;

        Vector3 pos = playerTarget.transform.position + new Vector3(randomPos.x, 0, randomPos.y);

        pos.y = areaCenter.position.y;

        if(Vector3.Distance(areaCenter.position, pos) > areaRadius)
        {
            return GetPosInArea();
        }

        return pos;
    }

    private void Emerge(Vector3 position)
    {
        //Vector3 pos = playerTarget.position + position;
        //pos.y = position.y;
        transform.position = position;

        if (digEnd)
        {
            digEnd.Raise();
        }
        MoveTimeCurrent = 0f;
        Debug.Log("Emerge!");
        animator.SetTrigger("ComeUp");
        state = MovingEnemyState.Idle;
    }

    private void Move()
    {
        MoveTimeCurrent = 0f;
        animator.SetTrigger("GoDown");
        if (digStart)
        {
            digStart.Raise();
        }
        Debug.Log("Move!");
        state = MovingEnemyState.Moving;
        
    }

    public void ReturnedToIdle()
    {
        //attackInitiated = false;
        state = MovingEnemyState.Idle;
    }

    public override void Attack()
    {
        animator.SetTrigger("Attack");
        //attackInitiated = true;
        state = MovingEnemyState.Anticipation;
        attackIntervalTimeCurrent = 0f;
        MoveTimeCurrent = 0f;

    }

    public void UpdateMeshVisibility(bool state)
    {
        //parasiteObject.SetActive(state);
    }

    protected override void Die()
    {
        OnDeath?.Invoke(this);
        StartCoroutine(DeathCoroutine());
    }
}

