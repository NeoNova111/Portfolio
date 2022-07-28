using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum ShootingType { FullAuto, SemiAuto, Charge }
public enum WeaponType { Food, Dart }

[RequireComponent(typeof(CinemachineImpulseSource))]
public abstract class BaseWeapon : MonoBehaviour
{
    public WeaponType WeaponType { get => weaponType; }
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected float rpm = 100f;
    protected float currentShotCooldown = 0f;
    [SerializeField] protected float impactDamage = 10f;
    public int MaxMagSizes { get => magazineSize; }
    public int RemainingMag { get => remainingMagazine; }
    public float ReloadTime { get => reloadTime; }
    [SerializeField] protected float reloadTime = 1f;
    protected float currentReloadTime;
    protected bool reloading = false;
    [SerializeField] protected int magazineSize = -1; //-1 for infinite mag
    protected int remainingMagazine;
    public int MaxAmmoReserves { get => maxAmmoReserves; }
    [SerializeField] protected int maxAmmoReserves = -1; //-1 for infinite mag
    protected int remainingAmmoReserves;
    [SerializeField] protected ShootingType shootingType = ShootingType.FullAuto;
    public ShootingType ShootingType { get => shootingType; }
    protected CinemachineImpulseSource cameraShaker;
    private bool equiped = false;
    public bool Equiped { get => equiped;  set => equiped = value; }
    [SerializeField] protected Animator animator;

    public abstract void Attack();
    public abstract void OnSwap();
    public abstract void AlternateAttack();
    public abstract void StopAttack();
    public abstract void Reload();
    public abstract void CancelReload();
    public abstract void BeginReload();
}
