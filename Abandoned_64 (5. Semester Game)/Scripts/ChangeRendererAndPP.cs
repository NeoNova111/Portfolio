using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRendererAndPP : MonoBehaviour
{
    private Camera cam;
    private UnityEngine.Rendering.Universal.UniversalAdditionalCameraData additionalCameraData;
    private bool debugModeisActive = false;
    private float currentAnimationValue;
    private float timeSinceDebugModeActivation = 0;

    public Material[] animateMaterialsWhenToggelingDebugMode;
    public float animationSpeed = 1f;
    public float debugClutterSpeedMultiplier = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        if (cam == null)
        {
            Debug.Log("This script should be attatched to a Camera");
        }
        additionalCameraData = cam.transform.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        additionalCameraData.SetRenderer(0);
    }

    void Update()
    {
        if (debugModeisActive)
        {
            currentAnimationValue += animationSpeed * Time.deltaTime;
            timeSinceDebugModeActivation += Time.deltaTime;
        }
        else
        {
            timeSinceDebugModeActivation = 0;
            currentAnimationValue -= animationSpeed * Time.deltaTime;
            if (currentAnimationValue <= 0)
            {
                additionalCameraData.SetRenderer(0);
            }
        }
        currentAnimationValue = Mathf.Clamp(currentAnimationValue, 0, 1);
        foreach (Material m in animateMaterialsWhenToggelingDebugMode)
        {
            m.SetFloat("Vector1_BeginningAnimation", currentAnimationValue);
            m.SetFloat("Vector1_TimeSinceDebugModeActivation", timeSinceDebugModeActivation * debugClutterSpeedMultiplier);
        }
    }

    public void ActivateNormalRenderer()
    {
        debugModeisActive = false;
    }

    public void ActivateDebugRenderer()
    {
        additionalCameraData.SetRenderer(1);
        debugModeisActive = true;
    }
}
