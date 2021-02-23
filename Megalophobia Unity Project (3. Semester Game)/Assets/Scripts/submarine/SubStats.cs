using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Submarine/Stats")]
public class SubStats : ScriptableObject
{
    public float maxHealth = 100;
    public float health = 100;

    public float forwardMomentum = 1f;
    public float strafeMomentum = 0.5f;
    public float backwardMomentum = 0.2f;
    public float diveMomentum = 1.75f;
    public float turnMomentum = 1f;
    public float sideturnMomentum = 3f;

    public float drag = 0;
    public float angulatDrag = 0.1f;

    public float collisionRepelForce = 10f;
    public float collisionInvulnTime = 0.5f;
    public float defaultCollisionDamage = 5f;

    public bool respawned = false;
    public float respawnTime = 3f;

    public float maxDamageLightIntensity = 500000f;
    public float maxDamageLightRange = 10f;

    public GameEvent gameOver;
    public GameEvent respawn;
    public GameEvent teleport;
    public GameEvent takeDamage;

    public Vector3 submarinePosition;
    public Vector3 submarineEulerAngles;
    public Quaternion submarineRotation;

    //public Transform subTransform;

    public bool TakeDamage(float damage)
    {
        takeDamage.Raise();
        health -= damage;
        if(health <= 0)
        {
            gameOver.Raise();
            return true;
        }
        return false;
    }
}
