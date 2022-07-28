using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowOverLifetime : MonoBehaviour
{
    public float growAmount = 10;
    public AnimationCurve growrate;
    public float totalLifeTime = 5;
    public ParticleSystemStopAction stopAction;
    private float currentLifetime = 0;
    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
    }

    private void Update()
    {
        if (currentLifetime == totalLifeTime) return;

        currentLifetime = Mathf.Clamp(currentLifetime + Time.deltaTime, 0, totalLifeTime);
        transform.localScale = startScale + Vector3.one * growrate.Evaluate(currentLifetime / totalLifeTime) * growAmount;

        if (currentLifetime == totalLifeTime)
        {
            switch (stopAction)
            {
                case ParticleSystemStopAction.None:
                    break;
                case ParticleSystemStopAction.Disable:
                    enabled = false;
                    break;
                case ParticleSystemStopAction.Destroy:
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }
    }

}
