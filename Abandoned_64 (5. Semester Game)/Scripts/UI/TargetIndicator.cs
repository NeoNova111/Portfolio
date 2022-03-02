using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{
    private CameraController cameraController;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        cameraController = CameraController.Instance;
    }

    void Update()
    {
        if (cameraController.LockOnTarget == null || cameraController.LockOnTarget.Equals(null))
            return;

        rectTransform.position = RectTransformUtility.WorldToScreenPoint(cameraController.MainCamera, cameraController.LockOnTarget.TargetTransform.position);
    }
}
