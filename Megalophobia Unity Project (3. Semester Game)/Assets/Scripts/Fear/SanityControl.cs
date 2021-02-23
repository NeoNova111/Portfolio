using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Cinemachine;

//decouple sanity pp using events
public class SanityControl : MonoBehaviour
{
    public Crewmember[] crewmembers; //for now keeping them
    public Volume sanityPostProcessing;
    public float transitionSpeed;

    Fear[] currentRoomsFears;
    float currentRoomFearIntensity;
    float fearDifference;

    public GameEvent VirtualCameraChanged;
    //CinemachineBrain cinemachineBrain;
    //CinemachineVirtualCamera activeVC;
    CameraControl cameraController;

    private void Start()
    {
        foreach(Crewmember crewmember in crewmembers)
            ResetSanity(crewmember);

        //cinemachineBrain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineBrain>();
        cameraController = CameraControl.instance;
    }

    private void ResetSanity(Crewmember crewmember)
    {
        crewmember.sanity = crewmember.maxSanity;
        crewmember.potentialFearIntensity = 0;
        sanityPostProcessing.weight = 0;
    }

    private void Update() //really ugly/ hacked with 3 for loops, look into comparing
    {
        if (currentRoomsFears != null)
        {
            foreach (Crewmember crewmember in crewmembers)
            {
                foreach (Fear fear in currentRoomsFears)
                {
                    foreach (FearType fearType in crewmember.typeOfFear)
                    {
                        if (fear.phobia == fearType)
                        {
                            crewmember.sanity -= fear.intensity * Time.deltaTime;
                        }
                    }
                }
            }

            if (sanityPostProcessing.weight < currentRoomFearIntensity)
            {
                sanityPostProcessing.weight += fearDifference / transitionSpeed * Time.deltaTime;
            }
            else if (sanityPostProcessing.weight > currentRoomFearIntensity)
            {
                if (sanityPostProcessing.weight - fearDifference / transitionSpeed * Time.deltaTime < 0)
                    sanityPostProcessing.weight = 0;
                else
                    sanityPostProcessing.weight -= fearDifference / transitionSpeed * Time.deltaTime;
            }

            SetCrewmemberCurrentFear(sanityPostProcessing.weight);
        }

        if (cameraController.HasActiveVC())
        {
            cameraController.SetVirtualCamShakeIntensity(sanityPostProcessing.weight);
        }
    }

    void SetCrewmemberCurrentFear(float intensity)
    {
        foreach(Crewmember crewmember in crewmembers)
        {
            crewmember.currentFearIntensity = intensity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Room")
        {
            currentRoomsFears = other.GetComponent<Room>().fears;
            currentRoomFearIntensity = GetRoomFearIntensity();
            fearDifference = Mathf.Abs(currentRoomFearIntensity - sanityPostProcessing.weight);
        }
    }

    float GetRoomFearIntensity()
    {
        float intensity = 0;
        foreach (Fear fear in currentRoomsFears)
        {
            intensity += fear.intensity;
        }

        if(intensity > 1)
        {
            return SetCrewFearIntensity(1);
        }
        return SetCrewFearIntensity(intensity);
    }

    float SetCrewFearIntensity(float intensity)
    {
        foreach(Crewmember member in crewmembers)
        {
            member.potentialFearIntensity = intensity;
        }

        return intensity;
    }

    //void SetVirtualCamShakeIntensity(float intensity)
    //{
    //    activeVC.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
    //}

    //IEnumerator NewVC()
    //{
    //    yield return null;
    //    activeVC = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
    //    VirtualCameraChanged.Raise();
    //}
}