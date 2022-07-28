using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CinemachineImpulseSourceActivator : MonoBehaviour
{
    public bool impulseOnEnable = false;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ActivateImpulse()
    {
        impulseSource.GenerateImpulse();
    }

    private void OnEnable()
    {
        if (impulseOnEnable) ActivateImpulse();
    }
}
