using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum BulletTravelType { Travel, HitScan }

public abstract class RangedWeapon : BaseWeapon
{
    [SerializeField] protected BulletTravelType travelType = BulletTravelType.Travel;
    [SerializeField] protected GameObject bullet;
    [SerializeField] protected GameObject bulletDeathEffect;
    [SerializeField] protected GameObject muzzleFlash;
    [SerializeField] protected Transform[] muzzleTransforms;
    protected int muzzleIdx = 0;
    [SerializeField] protected float windUpTime = 0f;
    protected IEnumerator windUpRoutine;
    protected bool woundUp = false;
    protected bool windingUp = false;
    protected bool canShoot = true;
    [SerializeField] protected int bulletCount = 1;
    [SerializeField] protected float maxRange = 20f;
    [SerializeField] protected float bulletSpeed = 10f;
    [SerializeField][Range(0, 1f)] protected float accuracy = 1f;
    [SerializeField] protected float knockbackForce = 0f;
    [SerializeField] protected LayerMask validHitscanLayers;
    //[SerializeField] protected float camerShake = 0f;

    [SerializeField] protected AK.Wwise.Event windUpSound;
    [SerializeField] protected AK.Wwise.Event continuousSound;
    [SerializeField] protected AK.Wwise.Event stopSound;
    [SerializeField] protected AK.Wwise.Event shotSound;

    private void Awake()
    {
        remainingMagazine = magazineSize;
    }

    private void Start()
    {
        //remainingAmmoReserves = Mathf.CeilToInt(maxAmmoReserves * WeaponsManager.Instance.RemainingReservesOfType(weaponType));
        cameraShaker = GetComponent<CinemachineImpulseSource>();
        muzzleIdx = 0;
    }

    private void OnEnable()
    {
        if(WeaponsManager.Instance)
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

    public override void Attack()
    {
        if (!canShoot) return;
        Shoot();
    }


    public override void AlternateAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void StopAttack()
    {
        if(windingUp) StopCoroutine(windUpRoutine);
        woundUp = false;
        windingUp = false;
        stopSound.Post(gameObject);
        canShoot = true;
    }

    public override void Reload()
    {
        if(maxAmmoReserves < 0 && remainingMagazine < magazineSize)
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

    public override void OnSwap()
    {
        CancelReload();
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

    public void Shoot()
    {
        if (!windingUp && !woundUp)
        {
            windUpRoutine = WindUpGun();
            StartCoroutine(windUpRoutine);
        }

        if (reloading) return;

        if (remainingMagazine > 0 && currentShotCooldown <= 0 && woundUp || magazineSize == -1 && currentShotCooldown <= 0 && woundUp)
        {
            currentShotCooldown = 1 / (rpm / 60f);
            remainingMagazine--;
            cameraShaker.GenerateImpulse();
            PlayerController playerInstance = PlayerController.Instance;
            playerInstance.ChangeForce(-transform.forward * knockbackForce);
            float totalDamage = impactDamage * playerInstance.stats.DamageMultiplier + playerInstance.stats.damageStat.TotalValue;
            Transform camTransform = Camera.main.transform;
            Vector3 nearClipPos = camTransform.position + camTransform.forward * Camera.main.nearClipPlane;

            for (int i = 0; i < bulletCount; i++)
            {
                //muzzleflash
                if (muzzleFlash) Instantiate(muzzleFlash, muzzleTransforms[muzzleIdx]);

                //calculate the raycast direction with regards to accuracy
                float finalAccuracy = 1 - accuracy;
                Vector3 castDirection = ((camTransform.up * Random.Range(-finalAccuracy, finalAccuracy) + camTransform.right * Random.Range(-finalAccuracy, finalAccuracy) + camTransform.forward)).normalized;

                //get the position of the bullets destination
                Vector3 targetPos;
                if (Physics.Raycast(camTransform.position, castDirection, out RaycastHit hit, maxRange, validHitscanLayers, QueryTriggerInteraction.Ignore)) targetPos = hit.point;
                else targetPos = camTransform.position + castDirection * maxRange;

                //project the weapon muzzle onto the cameras plane
                Vector3 muzzleCameraPlanePosition = Camera.main.WorldToScreenPoint(muzzleTransforms[muzzleIdx].position);

                float muzzleDistanceToNearClipPlane = Vector3.Distance(muzzleTransforms[muzzleIdx].position, nearClipPos);
                float targetDistanceToNearClipPlane = Vector3.Distance(targetPos, nearClipPos);
                float muzzleTotarget = targetDistanceToNearClipPlane - muzzleDistanceToNearClipPlane;

                float trueMuzzleDistance = muzzleDistanceToNearClipPlane;
                if(muzzleTotarget < muzzleDistanceToNearClipPlane)
                {
                    trueMuzzleDistance = Mathf.Clamp(muzzleDistanceToNearClipPlane - Mathf.Abs(muzzleTotarget - muzzleDistanceToNearClipPlane), Camera.main.nearClipPlane, muzzleDistanceToNearClipPlane);
                }

                Vector3 trueMuzzlePosition = Camera.main.ScreenToWorldPoint(new Vector3(muzzleCameraPlanePosition.x, muzzleCameraPlanePosition.y, trueMuzzleDistance));

                Vector3 shotDirection = (targetPos - trueMuzzlePosition).normalized;

                Quaternion bulletRotation;
                bulletRotation = Quaternion.LookRotation(camTransform.forward, Vector3.up);
                BaseBullet bulletClone;
                switch (travelType)
                {
                    case BulletTravelType.Travel:
                        bulletClone = Instantiate(bullet, trueMuzzlePosition, bulletRotation).GetComponent<BaseBullet>();
                        bulletClone.InitializeBullet(shotDirection * bulletSpeed, maxRange, totalDamage, bulletDeathEffect);
                        break;
                    case BulletTravelType.HitScan:
                        if (Physics.Raycast(camTransform.position, castDirection, out RaycastHit hitscan, maxRange, validHitscanLayers))
                        {
                            IDamagable hitDamagable = hitscan.transform.gameObject.GetComponent<IDamagable>();
                            if (hitDamagable != null) hitDamagable.TakeDamage(totalDamage);
                            Instantiate(bulletDeathEffect, hitscan.point, bulletRotation);
                        }
                        break;
                    default:
                        break;
                }

                //iterate muzzles
                muzzleIdx++;
                if (muzzleIdx >= muzzleTransforms.Length) muzzleIdx = 0;

                shotSound.Post(gameObject);
                if (shootingType == ShootingType.SemiAuto) canShoot = false;
                WeaponsManager.Instance.shootEvent.Raise();
            }
        }
        else if(remainingMagazine == 0)
        {
            BeginReload();
        }
    }

    protected IEnumerator WindUpGun()
    {
        windingUp = true;
        windUpSound.Post(gameObject);
        yield return new WaitForSeconds(windUpTime);
        continuousSound.Post(gameObject);
        windingUp = false;
        woundUp = true;
    }
}
