using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [System.Serializable]
    public struct WeaponSlot
    {
        public GameObject gameObject;
        public GameObject activeBackground;
    }

    [System.Serializable]
    public struct AmmoIcon
    {
        public Image fillImage;
        public GameObject activeBackground;
        public GameObject inactiveForeground;
        public WeaponType ammoType;
    }

    [SerializeField] private Image weaponIcon;
    //[SerializeField] private float ammoCount;
    [SerializeField] private TextMeshProUGUI ammoText;
    //[SerializeField] private float ammoreservesCount;
    [SerializeField] private TextMeshProUGUI ammoResevesText;
    [SerializeField] private WeaponSlot[] weaponSlots;
    [SerializeField] private AmmoIcon[] icons;

    private void Start()
    {
        //UpdateWeaponUI();
        //UpdateAmmoUI();
    }

    public void UpdateWeaponUI()
    {
        //todo get rid of singlton approach
        WeaponsManager weaponManager = WeaponsManager.Instance;
        ActivateOccupiedSlots();
        for (int i = 0; i < weaponManager.WeaponInventory.Length; i++)
        {
            if (i == weaponManager.WeaponIndex)
            {
                weaponSlots[i].activeBackground.SetActive(true);
                for (int j = 0; j < icons.Length; j++)
                {
                    if (icons[j].ammoType == weaponManager.CurrentWeapon.WeaponType && weaponManager.CurrentWeapon.MaxAmmoReserves >= 0)
                    {
                        icons[j].activeBackground.SetActive(true);
                        icons[j].inactiveForeground.SetActive(false);
                    }
                    else
                    {
                        icons[j].activeBackground.SetActive(false);
                        icons[j].inactiveForeground.SetActive(true);
                    }
                }
            }
            else
            {
                weaponSlots[i].activeBackground.SetActive(false);
            }
        }

        weaponIcon.sprite = weaponManager.WeaponInventory[weaponManager.WeaponIndex].GetComponent<WeaponPickup>().WeaponSprite; //todo also wonky rn, implement mor eexplicit way to set icon
    }

    public void UpdateAmmoUI()
    {
        WeaponsManager weaponManager = WeaponsManager.Instance;
        if (weaponManager.CurrentWeapon.MaxAmmoReserves < 0) ammoResevesText.text = "/∞";
        else ammoResevesText.text = "/" + Mathf.CeilToInt(weaponManager.RemainingReservesOfCurrentWeapon() * weaponManager.CurrentWeapon.MaxAmmoReserves);

        if (weaponManager.CurrentWeapon.MaxMagSizes < 0) ammoText.text = "∞";
        else ammoText.text = "" + weaponManager.CurrentWeapon.RemainingMag;


        foreach (var v in icons)
        {
            v.fillImage.fillAmount = WeaponsManager.Instance.RemainingReservesOfType(v.ammoType);
        }
    }

    private void ActivateOccupiedSlots()
    {
        WeaponsManager weaponManager = WeaponsManager.Instance;
        for (int i = 0; i < weaponManager.WeaponInventory.Length; i++)
        {
            if (weaponManager.WeaponInventory[i]) weaponSlots[i].gameObject.SetActive(true);
            else weaponSlots[i].gameObject.SetActive(false);
        }
    }
}
