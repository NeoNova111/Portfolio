using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MeleeWeapon : BaseWeapon
{
    [SerializeField] private AK.Wwise.Event attackSound;
    [SerializeField] private GameObject attackColliders;
    private DamagingCollider damageColl;
    private float speedMultipier;
    private bool alternateAttackActive = false;
    private IEnumerator alternateAttackCoroutine;
    [SerializeField] protected LayerMask validHitLayers;

    private void Start()
    {
        cameraShaker = GetComponent<CinemachineImpulseSource>();
        damageColl = GetComponentInChildren<DamagingCollider>();
        damageColl.Damage = impactDamage;
        DeactivateAttack();
        animator.SetFloat("speedMultiplier", rpm / 60f);
    }

    private void OnEnable()
    {
        if (WeaponsManager.Instance)
            remainingAmmoReserves = Mathf.CeilToInt(maxAmmoReserves * WeaponsManager.Instance.RemainingReservesOfType(weaponType));
    }

    public void Update()
    {
        currentShotCooldown -= Time.deltaTime;

        //spinny weapon while reloading
        if (reloading)
        {
            if (currentReloadTime <= 0)
            {
                Reload();
                transform.rotation = transform.parent.rotation;
            }
            else
            {
                currentReloadTime -= Time.deltaTime;
                transform.RotateAround(transform.position, transform.right, (-360 / reloadTime) * Time.deltaTime);
            }
        }
    }

    public override void AlternateAttack()
    {
        if (alternateAttackActive) return;

        alternateAttackCoroutine = AlternateAttackRoutine();
        StartCoroutine(alternateAttackCoroutine);
    }

    public override void Attack()
    {
        Swing();
    }

    private void Swing()
    {
        if (reloading) return;

        if (remainingMagazine > 0 && currentShotCooldown <= 0/* && woundUp */|| magazineSize == -1 && currentShotCooldown <= 0 /*&& woundUp*/)
        {
            currentShotCooldown = 1 / (rpm / 60f);
            remainingMagazine--;

            ActivateAttack();
            animator.SetTrigger("Attack");

            attackSound.Post(gameObject);
            WeaponsManager.Instance.shootEvent.Raise();
        }
        else if (remainingMagazine == 0)
        {
            BeginReload();
        }
    }

    public override void BeginReload()
    {
        remainingAmmoReserves = Mathf.CeilToInt(maxAmmoReserves * WeaponsManager.Instance.RemainingReservesOfType(weaponType)); //check if ammo was picked up/ if reserves increased
        if (magazineSize < 0 || remainingMagazine == magazineSize || remainingAmmoReserves == 0 && maxAmmoReserves >= 0) return; //don't even try reload if not possible

        currentShotCooldown = 0;
        currentReloadTime = reloadTime;
        reloading = true;
    }

    public override void CancelReload()
    {
        transform.rotation = transform.parent.rotation;
        reloading = false;
    }

    public override void Reload()
    {
        if (maxAmmoReserves < 0 && remainingMagazine < magazineSize)
        {
            reloading = false;
            remainingMagazine = magazineSize;
            WeaponsManager.Instance.reloadEvent.Raise();
            return;
        }

        int toReload = magazineSize - remainingMagazine;
        toReload = Mathf.Clamp(toReload, 0, remainingAmmoReserves);
        reloading = false;

        if (toReload == 0) return;

        remainingMagazine += toReload;
        remainingAmmoReserves -= toReload;
        WeaponsManager.Instance.CalcRemainingReserves(maxAmmoReserves, remainingAmmoReserves, weaponType);
        WeaponsManager.Instance.reloadEvent.Raise();
    }

    public void ActivateAttack()
    {
        attackColliders.SetActive(true);
    }

    public void DeactivateAttack()
    {
        attackColliders.SetActive(false);
    }

    public override void StopAttack()
    {
        
    }

    public override void OnSwap()
    {
        CancelReload();
        if(alternateAttackCoroutine != null)
        {
            StopCoroutine(alternateAttackCoroutine);
            alternateAttackActive = false;
        }
    }

    private IEnumerator AlternateAttackRoutine()
    {
        alternateAttackActive = true;
        animator.SetTrigger("Alternate");

        EnemyHierarchicalStateMachine hitEnemy = null;
        Transform cameraTransform = Camera.main.transform;
        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, 5f, validHitLayers, QueryTriggerInteraction.Ignore))
        {
            hitEnemy = hit.transform.root.GetComponent<EnemyHierarchicalStateMachine>();
        }

        if (hitEnemy)
        {
            hitEnemy.navmeshagent.isStopped = true;
            Vector3 targetPos = PlayerController.Instance.transform.position + PlayerController.Instance.transform.forward;

            while (true)
            {
                hitEnemy.transform.position = Vector3.MoveTowards(hitEnemy.transform.position, targetPos, 1f * Time.deltaTime);
                yield return new WaitForEndOfFrame();
                if(hitEnemy.transform.position == targetPos)
                {
                    break;
                }
            }

            hitEnemy.navmeshagent.isStopped = false;
        }
        else
        {
            //returning lasso
            yield return new WaitForSeconds(1f);
        }

        alternateAttackActive = false;
    }
}
