using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraEventEffects : MonoBehaviour
{
    CameraControl cameraController;

    void Start()
    {
        cameraController = CameraControl.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetShakeIntensity(float intensity)
    {
        cameraController.GetActiveVC().GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
    }

    public void ShakeCamera(float intensity)
    {
        cameraController.GetActiveVC().GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
    }
}