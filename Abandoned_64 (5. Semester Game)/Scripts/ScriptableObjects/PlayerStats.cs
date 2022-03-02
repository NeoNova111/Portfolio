using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [SerializeField] private GameEvent statsChanged;
    private Transform playerTransform;
    [Range(0,100)] public int maxHealth;
    [Range(0,100)] private float currentHealth;
    public int collectibleCount;
    public int keyCount;
    public bool allowedToUseDebug = false;
    public bool respawning = false;

    public GameEvent StatsChanged { get => statsChanged; }
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
}
