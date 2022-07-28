using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleImpulse : MonoBehaviour
{
    [SerializeField] private float minScale= 0f;
    [SerializeField] private float maxScale = 5f;
    [SerializeField] private float maxDps = 50f;
    [SerializeField] private AnimationCurve hitmarkerOverLifetimeCurve;
    [SerializeField] private float hitmarkerLifeTime = 0.2f;

    private RectTransform rect;
    private float currentDps;
    private float currentLifetime;
    private float damageScaleValue;

    private void Start()
    {
        currentLifetime = hitmarkerLifeTime;
        damageScaleValue = 0f;
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(currentDps > 0)
        {
            currentDps = Mathf.Clamp(currentDps - Time.deltaTime * maxDps, 0, maxDps);
        }

        if (currentLifetime < hitmarkerLifeTime)
        {
            currentLifetime = Mathf.Clamp(currentLifetime + Time.deltaTime, 0, hitmarkerLifeTime);
            float scaleValue = hitmarkerOverLifetimeCurve.Evaluate(currentLifetime / hitmarkerLifeTime) * damageScaleValue;
            rect.localScale = Vector3.one * scaleValue;
        }
    }

    public void AddCurrentDamage(float dmg)
    {
        currentDps = Mathf.Clamp(currentDps + dmg, 0, maxDps);
        currentLifetime = 0f;
        damageScaleValue = Mathf.Clamp((currentDps / maxDps) * maxScale, minScale, maxScale);
    }
}
