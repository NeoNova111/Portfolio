using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Image health;
    public Color healthColor;
    public Color pauseColor;

    private void Start()
    {
        ChangeDamagable(true);
        SetHealth(1f);
    }

    public void SetHealth(float percentage)
    {
        health.fillAmount = percentage;
    }

    public void ChangeDamagable(bool damagable)
    {
        if (damagable)
        {
            health.color = healthColor;
        }
        else
        {
            health.color = pauseColor;
        }
    }
}
