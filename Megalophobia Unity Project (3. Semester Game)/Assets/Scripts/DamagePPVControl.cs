using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DamagePPVControl : MonoBehaviour
{
    public Volume parasitePPV;
    [Range(0, 1)]
    public float maxWeight = 1;
    public float delay = 0.0f;
    public float sustain = 0.3f;
    public float decay = 0.7f;
    float delayTime;
    float sustainTime;
    float decayTime;

    private void Start()
    {
        delayTime = delay;
        sustainTime = sustain;
        decayTime = decay;
    }

    private void Update()
    {
        float strength = 1 - (submarineMovement.instance.submarineStats.health / submarineMovement.instance.submarineStats.maxHealth);
        if (delayTime < delay)
        {
            delayTime += Time.deltaTime;
        }
        else if (sustainTime < sustain)
        {
            if (parasitePPV.weight != 1)
                parasitePPV.weight = strength;

            sustainTime += Time.deltaTime;
        }
        else if (decayTime < decay)
        {
            parasitePPV.weight = Mathf.Lerp(strength, 0, decayTime / decay);
            decayTime += Time.deltaTime;
        }
        else if (parasitePPV.weight != 0)
        {
            parasitePPV.weight = 0;
        }
    }

    public void TakeDmg()
    {
        delayTime = 0;
        sustainTime = 0;
        decayTime = 0;
    }
}
