using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    public GameObject scannerUI;
    public Image scanCooldownImage;

    public GameObject divingPrompt;

    float scanCooldown;
    float maxScanCooldown;

    public void StartScanCooldown(float cd)
    {
        maxScanCooldown = cd;
        scannerUI.SetActive(true);
        scanCooldown = maxScanCooldown;
        scanCooldownImage.fillAmount = 1;
    }

    public void ResetScanCooldown()
    {
        scanCooldown = -1;
    }

    private void Update()
    {
        if(scanCooldown > 0)
        {
            scanCooldown -= Time.deltaTime;
            scanCooldownImage.fillAmount = scanCooldown / maxScanCooldown;
        }

        if (scanCooldown <= 0)
            scannerUI.SetActive(false);
    }

    public void SetMaxScanCooldown(float cooldown)
    {
        maxScanCooldown = cooldown;
    }

    public void ShowDivingPrompt(bool b)
    {
        divingPrompt.SetActive(b);
    }
}
