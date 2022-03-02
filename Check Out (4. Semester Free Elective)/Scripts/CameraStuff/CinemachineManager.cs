using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineManager : MonoBehaviour
{
    CinemachineManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineVirtualCamera vcam_pc;
    [SerializeField] private CinemachineVirtualCamera vcam_queue;
    [SerializeField] private CinemachineVirtualCamera vcam_dialogue;
    private CinemachineVirtualCamera active;
    private CinemachineVirtualCamera prevActive;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetUpVCs());
    }

    IEnumerator SetUpVCs()
    {
        yield return new WaitForEndOfFrame();
        active = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        prevActive = active;
        if (GameManager.instance.cameFromGame)
        {
            SwitchToDialogueCamera();
        }
    }

    void SwitchToVirtualCamera(CinemachineVirtualCamera vc)
    {
        vcam_pc.Priority = 0;
        vcam_dialogue.Priority = 0;
        vcam_queue.Priority = 0;

        vc.Priority = 1;
        prevActive = active;
        active = vc;
    }

    public void SwitchBackToPreviousVirtualCamera()
    {
        SwitchToVirtualCamera(prevActive);
    }

    public void SwitchToPCCamera()
    {
        SwitchToVirtualCamera(vcam_pc);
    }

    public void SwitchToQueueCamera()
    {
        SwitchToVirtualCamera(vcam_queue);
    }

    public void SwitchToDialogueCamera()
    {
        SwitchToVirtualCamera(vcam_dialogue);
    }
}
