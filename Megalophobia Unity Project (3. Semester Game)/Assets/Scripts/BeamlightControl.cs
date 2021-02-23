using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamlightControl : MonoBehaviour
{
    public Light[] lights;
    public float povLightStrengthInLumen;
    public float transitionTime;

    float previousLightStrength;
    float toStart;
    float fromStart;
    float toTime;
    float fromTime;

    submarineMovement subinstance;
    

    // Start is called before the first frame update
    void Start()
    {
        subinstance = submarineMovement.instance;
        previousLightStrength = lights[0].intensity;
        toStart = lights[0].intensity;
        fromStart = lights[0].intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if(subinstance.GetCurrentRoom().perspective == CameraPerspective.POV)
        {
            TransitionLightIntensityToPOV(toStart, povLightStrengthInLumen);
        }
        else
        {
            TransitionLightIntensityFromPOV(fromStart, previousLightStrength);
        }
    }

    void TransitionLightIntensityToPOV(float startValue, float targetValue)
    {
        fromTime = 0;
        toTime += Time.deltaTime;

        if (toTime > transitionTime)
            toTime = transitionTime;

        foreach(Light light in lights)
        {
            light.intensity = Mathf.Lerp(startValue, targetValue, toTime/ transitionTime);
        }

        fromStart = lights[0].intensity;
    }

    void TransitionLightIntensityFromPOV(float startValue, float targetValue)
    {
        toTime = 0;
        fromTime += Time.deltaTime;

        if (fromTime > transitionTime)
            fromTime = transitionTime;

        foreach(Light light in lights)
        {
            light.intensity = Mathf.Lerp(startValue, targetValue, fromTime/ transitionTime);
        }

        toStart = lights[0].intensity;
    }
}
