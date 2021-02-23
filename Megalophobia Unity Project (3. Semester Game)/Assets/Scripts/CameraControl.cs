using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum CameraPerspective { TOPDOWN, SIDE, POV };

public class CameraControl : MonoBehaviour
{
    #region Singelton

    public static CameraControl instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null)
        {
            Debug.LogWarning("More than one camera instance");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    #endregion

    #region vars

    CinemachineBrain Brain;
    CinemachineVirtualCamera activeVC;
    CinemachineVirtualCamera previousVC;

    CameraPerspective perspective;

    Transform target;
    Transform prevTarget;

    public GameEvent VirtualCameraChanged;
    public GameEvent PerspectiveChanged;

    #endregion

    private void Start()
    {
        Brain = GetComponent<CinemachineBrain>();
    }

    private void LateUpdate()
    {
        if (Brain.IsBlending)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    #region Utility

    public void InstantBlend()
    {
        StartCoroutine(FlickerBrain()); //workaround
    }

    IEnumerator FlickerBrain()
    {
        Brain.enabled = false;

        yield return null;

        Brain.enabled = true;
    }

    #endregion

    #region GetterSetter

    public void SetBlendTime(float time)
    {
        Brain.m_DefaultBlend.m_Time = time;
    }

    public float GetBlendTime()
    {
        return Brain.m_DefaultBlend.m_Time;
    }

    public void SetActiveVC(CinemachineVirtualCamera vc)
    {
        activeVC = vc;
    }

    public void SetActiveVC()
    {
        StartCoroutine("NewVC");
    }

    public CinemachineVirtualCamera GetActiveVC()
    {
        return activeVC;
    }

    //maybe implement using priorities rater than deactivating
    IEnumerator NewVC()
    {
        yield return null;
        activeVC = Brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        VirtualCameraChanged.Raise();
    }

    public void SetVirtualCamShakeIntensity(float intensity)
    {
        activeVC.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
    }

    public bool HasActiveVC()
    {
        return activeVC != null;
    }

    public void SetTransitionTime(float time)
    {
        Brain.m_DefaultBlend.m_Time = time;
    }

    public float GetTransitionTime()
    {
        return Brain.m_DefaultBlend.m_Time;
    }

    public void SetPreviousVC(CinemachineVirtualCamera prev)
    {
        previousVC = prev;
    }

    public CinemachineVirtualCamera GetPreviousVC()
    {
        return previousVC;
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void PrevSetTarget(Transform t)
    {
        prevTarget = t;
    }

    public Transform GetPrevTarget()
    {
        return prevTarget;
    }

    public bool IsBlending()
    {
        return Brain.IsBlending;
    }
    #endregion
}
