using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    private Enemy enemy;

    private void Start()
    {
        //slider = gameObject.GetComponent<Slider>();
    }

    private void Update()
    {
        SetHealth(enemy.currentHealth);
    }

    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        SetHealth(maxHealth);
    }

    public void SetHealth(float healt)
    {
        slider.value = healt;
    }

    public void SetEnemy(Enemy enemyRef)
    {
        enemy = enemyRef;
    }
}
