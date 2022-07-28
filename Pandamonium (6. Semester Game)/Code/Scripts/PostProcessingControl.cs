using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingControl : MonoBehaviour
{
    public static PostProcessingControl Instance { get => instance; }
    private static PostProcessingControl instance;

    [SerializeField, Range(0, 1f)] private float maxVignetteIntensity = 0.6f;
    [SerializeField] private AnimationCurve vignetteIntensity;
    [SerializeField] private AnimationCurve vignettePulse;
    [SerializeField] private float pulseTime = 0.5f;
    [SerializeField] private float pulseFrequency = 3f;
    private Color pulseColor;
    private Color defaultColor;
    private float currentPulseTime;
    private float currentValue;
    private float currentVignette;

    private Volume ppv;
    private Vignette damageVignette;
    private ChromaticAberration chromatic;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); //only one instance of singleton allowed
        }

        ppv = GetComponent<Volume>();
        ppv.profile.TryGet(out damageVignette);
        ppv.profile.TryGet(out chromatic);

        defaultColor = damageVignette.color.value;
        pulseColor = defaultColor;

        chromatic.intensity.value = 0;
        currentVignette = 0;
        damageVignette.intensity.value = currentVignette;
    }

    private void Start()
    {
        currentPulseTime = pulseTime;
    }

    private void Update()
    {
        if(currentPulseTime < pulseTime)
        {
            currentPulseTime = Mathf.Clamp(currentPulseTime + Time.deltaTime, 0, pulseTime);

            //do pulse
            float t = currentPulseTime / pulseTime;
            damageVignette.color.value = Color.Lerp(pulseColor, defaultColor, t);
            currentVignette = currentValue * vignettePulse.Evaluate(t);
            chromatic.intensity.value = vignettePulse.Evaluate(t);

        }
        else
        {
            damageVignette.color.value = defaultColor;
            chromatic.intensity.value = 0;
        }

        damageVignette.intensity.value = currentVignette * SineBetween(0.9f, 1.1f, Time.timeSinceLevelLoad);
    }

    public void VignettePulse(float healthPercentage, Color pulseColor)
    {
        currentPulseTime = 0f;
        currentValue = healthPercentage;
        this.pulseColor = pulseColor;

        healthPercentage = Mathf.Clamp01(healthPercentage);
        currentValue = vignetteIntensity.Evaluate(healthPercentage) * maxVignetteIntensity;
    }

    public float SineBetween(float min, float max, float t)
    {
        var halfRange = (max - min) / 2;
        return min + halfRange + Mathf.Sin(t * pulseFrequency) * halfRange;
    }
}
