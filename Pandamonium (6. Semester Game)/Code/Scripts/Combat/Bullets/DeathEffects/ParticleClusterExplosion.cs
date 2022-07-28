using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleClusterExplosion : MonoBehaviour
{
    [Header("Cluster")]
    [SerializeField][Range(0f, 1f)] public float triggerChance = 0.1f;
    [SerializeField][Range(0f, 1f)] private float triggerChanceModifier = 0.5f;
    private float trueTriggerChance;
    [SerializeField] private GameObject clusterExplosion;
    [SerializeField] private float scaleModifier = 0.5f;
    [SerializeField] private float minScale = 0.1f;
    [SerializeField] public int remainingTriggers = 2;

    private ParticleSystem ps;
    private List<ParticleSystem.Particle> exit = new List<ParticleSystem.Particle>();

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        int particleCount = ps.emission.GetBurst(0).maxCount;
        float averageExplosions = particleCount * triggerChance;
        float expectedNextExplosions = averageExplosions * triggerChanceModifier;
        trueTriggerChance = expectedNextExplosions / (averageExplosions * particleCount);
    }

    private void OnParticleTrigger()
    {
        int numExit = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);

        // iterate through the particles which exited the trigger and make them green
        for (int i = 0; i < numExit; i++)
        {
            bool cluster = Random.Range(0f, 1f) <= triggerChance;
            if (cluster && remainingTriggers > 0)
            {
                ParticleSystem.Particle p = exit[i];
                GameObject explosionObject = Instantiate(clusterExplosion, p.position, Quaternion.identity); //make sure simulated world space is set to world
                ParticleClusterExplosion explosion = explosionObject.GetComponent<ParticleClusterExplosion>();
                explosion.triggerChance = trueTriggerChance;
                explosion.remainingTriggers--;
                explosionObject.transform.localScale = new Vector3( Mathf.Clamp(explosionObject.transform.localScale.x * scaleModifier, minScale, 1),
                                                                    Mathf.Clamp(explosionObject.transform.localScale.y * scaleModifier, minScale, 1),
                                                                    Mathf.Clamp(explosionObject.transform.localScale.z * scaleModifier, minScale, 1));
                p.remainingLifetime = 0;
                exit[i] = p;
            }
        }

        // re-assign the modified particles back into the particle system
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);
    }
}
