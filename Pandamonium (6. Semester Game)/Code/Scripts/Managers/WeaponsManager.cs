using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    public static WeaponsManager Instance { get => instance; }
    private static WeaponsManager instance;

    [System.Serializable]
    public struct AmmoReserves
    {
        public WeaponType type;
        public Sprite reserveIcon;
        [Range(0, 1)] public float remainingReservePercentage;
    }

    [Header("Weaponry")]
    [SerializeField] private BaseWeapon defaultWeapon;
    [SerializeField] private Transform holdingHand;
    private int currentWeaponIndex = 0;
    public int WeaponIndex { get => currentWeaponIndex; }
    private BaseWeapon[] weaponInventory = new BaseWeapon[3];
    public BaseWeapon[] WeaponInventory { get => weaponInventory; }
    public BaseWeapon CurrentWeapon { get => weaponInventory[currentWeaponIndex]; }
    public AmmoReserves[] ammoReserves;
    public GameEvent equipEvent;
    public GameEvent ammoEvent;
    public GameEvent reloadEvent;
    public GameEvent shootEvent;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    private void Start()
    {
        weaponInventory[0] = defaultWeapon;
        EquipWeapon(weaponInventory[0]);
    }

    private void Update()
    {
        HandleWeapons();
    }

    private void HandleWeapons()
    {
        if (Input.GetKeyDown(KeyCode.R)) CurrentWeapon.BeginReload();

        if (Input.GetKeyDown(KeyCode.Q)) DropWeapon(true);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeEquipSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeEquipSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeEquipSlot(3);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            NextWeapon();
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            PrevWeapon();
        }

        if (weaponInventory[currentWeaponIndex])
        {
            switch (weaponInventory[currentWeaponIndex].ShootingType)
            {
                //both are same for now
                case ShootingType.FullAuto:
                    if (Input.GetMouseButton(0))
                        weaponInventory[currentWeaponIndex].Attack();
                    else if (Input.GetMouseButtonUp(0))
                        weaponInventory[currentWeaponIndex].StopAttack();

                    if (Input.GetMouseButton(1))
                        weaponInventory[currentWeaponIndex].AlternateAttack();
                    break;
                case ShootingType.SemiAuto:
                    if (Input.GetMouseButton(0))
                        weaponInventory[currentWeaponIndex].Attack();
                    else if (Input.GetMouseButtonUp(0))
                        weaponInventory[currentWeaponIndex].StopAttack();

                    if (Input.GetMouseButton(1))
                        weaponInventory[currentWeaponIndex].AlternateAttack();
                    break;
                default:
                    break;
            }
        }
    }

    public void AddToWeaponInventory(BaseWeapon weapon)
    {
        //check for empty slots
        for (int i = 1; i < weaponInventory.Length; i++)
        {
            if (!weaponInventory[i])
            {
                UneqiupWeapon();
                currentWeaponIndex = i;
                weaponInventory[currentWeaponIndex] = weapon;
                EquipWeapon(weapon);
                StartCoroutine(AlignToParent(weaponInventory[currentWeaponIndex].transform, 6f, 720f));
                return;
            }
        }

        if (currentWeaponIndex == 0)
        {
            NextWeapon();
        }

        DropWeapon();
        weaponInventory[currentWeaponIndex] = weapon;
        EquipWeapon(weapon);
        StartCoroutine(AlignToParent(weaponInventory[currentWeaponIndex].transform, 6f, 720f));
    }

    public void EquipWeapon(BaseWeapon weapon)
    {
        weaponInventory[currentWeaponIndex] = weapon;
        weaponInventory[currentWeaponIndex].gameObject.SetActive(true);
        weaponInventory[currentWeaponIndex].Equiped = true;
        weaponInventory[currentWeaponIndex].GetComponent<WeaponPickup>().enabled = false;
        weaponInventory[currentWeaponIndex].transform.parent = holdingHand;
        equipEvent.Raise();
    }

    public void ChangeEquipSlot(int slot)
    {
        slot = Mathf.Clamp(slot - 1, 0, weaponInventory.Length - 1);
        if (weaponInventory[slot])
        {
            if (weaponInventory[currentWeaponIndex]) UneqiupWeapon();
            currentWeaponIndex = slot;
            EquipWeapon(weaponInventory[currentWeaponIndex]);
        }
    }

    public void UneqiupWeapon()
    {
        weaponInventory[currentWeaponIndex].OnSwap();
        weaponInventory[currentWeaponIndex].gameObject.SetActive(false);
    }

    public void NextWeapon()
    {
        if (weaponInventory[currentWeaponIndex]) UneqiupWeapon();

        do
        {
            currentWeaponIndex++;
            if (currentWeaponIndex == weaponInventory.Length) currentWeaponIndex = 0;
        } while (weaponInventory[currentWeaponIndex] == null);

        EquipWeapon(weaponInventory[currentWeaponIndex]);
    }

    public void PrevWeapon()
    {
        if (weaponInventory[currentWeaponIndex]) UneqiupWeapon();

        do
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0) currentWeaponIndex = weaponInventory.Length - 1;
        } while (weaponInventory[currentWeaponIndex] == null);

        EquipWeapon(weaponInventory[currentWeaponIndex]);
    }

    public void DropWeapon(bool cycleNext = false)
    {
        if (currentWeaponIndex == 0) return; //can't drop default weapon

        if (weaponInventory[currentWeaponIndex])
        {
            weaponInventory[currentWeaponIndex].CancelReload();
            weaponInventory[currentWeaponIndex].GetComponent<WeaponPickup>().enabled = true;
            weaponInventory[currentWeaponIndex].transform.parent = null;
            weaponInventory[currentWeaponIndex].GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10f, ForceMode.Impulse);
            weaponInventory[currentWeaponIndex].Equiped = false;
            weaponInventory[currentWeaponIndex] = null;
            if (cycleNext) NextWeapon();
            return;
        }

    }

    public IEnumerator AlignToParent(Transform toAlign, float stepSize, float rotationDelta)
    {
        while (toAlign.parent != null && (toAlign.rotation != toAlign.parent.rotation || toAlign.position != toAlign.parent.position))
        {
            toAlign.position = Vector3.MoveTowards(toAlign.position, toAlign.parent.position, stepSize * Time.deltaTime);
            toAlign.rotation = Quaternion.RotateTowards(toAlign.rotation, toAlign.parent.rotation, rotationDelta * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public void RefillReserves(WeaponType type, float percentageAmount)
    {
        for(int i = 0; i < ammoReserves.Length; i++)
        {
            if (ammoReserves[i].type == type) ammoReserves[i].remainingReservePercentage = Mathf.Clamp01(ammoReserves[i].remainingReservePercentage + percentageAmount);
            ammoEvent.Raise();
        }
    }

    public float RemainingReservesOfType(WeaponType type)
    {
        for (int i = 0; i < ammoReserves.Length; i++)
        {
            if (ammoReserves[i].type == type) return ammoReserves[i].remainingReservePercentage;
        }

        return -1f;
    }

    public float RemainingReservesOfCurrentWeapon()
    {
        return RemainingReservesOfType(weaponInventory[currentWeaponIndex].WeaponType);
    }

    public void CalcRemainingReserves(float max, float current, WeaponType type)
    {
        for (int i = 0; i < ammoReserves.Length; i++)
        {
            if (ammoReserves[i].type == type) ammoReserves[i].remainingReservePercentage = current / max;
        }
    }
}