using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private Image Health;
    [SerializeField] private float health;
   
    // Start is called before the first frame update
    void Start()
    {
        Health = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        health = (float)playerStats.CurrentHealth/(float)playerStats.maxHealth;
        Health.fillAmount = health;
    }
}
