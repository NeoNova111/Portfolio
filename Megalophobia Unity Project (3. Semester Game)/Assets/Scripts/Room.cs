using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Room : MonoBehaviour
{
    public Fear[] fears;

    public BoxCollider colliderArea;
    //public SubStats submarineStats;
    public CinemachineVirtualCamera topdownVC;
    public CinemachineVirtualCamera sideviewVC;
    public CinemachineVirtualCamera POV_VC;

    public float cameraBlendTime = 0.5f;
    public CameraPerspective perspective;

    [Tooltip("Only relevant for pov and/ or topdown")]
    public Vector3 cameraOffset = new Vector3(0, 0, 0);
    [Tooltip("Only relevant for sideview")]
    public float cameraDistance = 0;

    CameraControl cameraController;
    Coroutine deactivatePrevCoroutine;

    bool priorityIncreasedOnce = false;
    bool blending = false;

    private void Start()
    {
        if (CameraControl.instance == null)
        {
            Debug.LogWarning("This Scene needs a CameraControl instance/ MainCamera prefab");
            return;
        }

        SetupCamPosition();
        VCamSettings();
        cameraController = CameraControl.instance;

        //StartCoroutine("InitialSetup");
    }

    private void Update()
    {
        if(perspective == CameraPerspective.TOPDOWN && this == submarineMovement.instance.GetCurrentRoom())
        {
            if (!cameraController.IsBlending())
            {
                if(topdownVC.LookAt != submarineMovement.instance.transform)
                    topdownVC.LookAt = submarineMovement.instance.transform;
            }
            else
            {
                if(topdownVC.LookAt != null)
                    topdownVC.LookAt = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = colliderArea.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(colliderArea.transform.position + colliderArea.center - transform.position, colliderArea.size);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (submarineMovement.instance.GetCurrentRoom() && submarineMovement.instance.GetCurrentRoom().perspective == CameraPerspective.TOPDOWN)
            {
                cameraController.GetActiveVC().LookAt = null;
            }

            deactivatePrevCoroutine = StartCoroutine(SetUpRoomCams());
            submarineMovement.instance.SetCurrentRoom(this);

            if(perspective == CameraPerspective.SIDE)
            {
                submarineMovement.instance.SideViewSetup(cameraBlendTime);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            cameraController.SetPreviousVC(cameraController.GetActiveVC());
            StopCoroutine(deactivatePrevCoroutine);
        }
    }

    void SetupCamPosition()
    {
        switch(perspective)
        {
            case CameraPerspective.TOPDOWN:
                topdownVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = cameraOffset;
                break;
            case CameraPerspective.SIDE:
                sideviewVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = transform.right.normalized * cameraDistance;
                break;
            case CameraPerspective.POV:
                POV_VC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = cameraOffset;
                break;
        }
    }

    void VCamSettings()
    {
        topdownVC.Follow = submarineMovement.instance.transform;
        topdownVC.LookAt = null;

        if (GameObject.FindGameObjectWithTag("SubLookAt"))
        {
            sideviewVC.Follow = GameObject.FindGameObjectWithTag("SubLookAt").transform;
            sideviewVC.LookAt = GameObject.FindGameObjectWithTag("SubLookAt").transform;
        }
        else
            Debug.LogError("LookAT prefab needed with submarine, otherwise certain cameras don't work");

        POV_VC.Follow = submarineMovement.instance.transform;
        POV_VC.LookAt = submarineMovement.instance.transform;
    }

    IEnumerator SetUpRoomCams()
    {
        yield return null;
        switch (perspective)
        {
            case CameraPerspective.TOPDOWN:
                topdownVC.gameObject.SetActive(true);
                cameraController.SetActiveVC(topdownVC);
                break;
            case CameraPerspective.SIDE:
                sideviewVC.gameObject.SetActive(true);
                cameraController.SetActiveVC(sideviewVC);
                break;
            case CameraPerspective.POV:
                POV_VC.gameObject.SetActive(true);
                cameraController.SetActiveVC(POV_VC);
                break;
        }

        cameraController.SetTransitionTime(cameraBlendTime);

        if (!priorityIncreasedOnce)
        {
            priorityIncreasedOnce = true;
            cameraController.GetActiveVC().m_Priority++;
        }

        if(cameraController.GetPreviousVC() != null && cameraController.GetActiveVC() != cameraController.GetPreviousVC())
        {
            yield return new WaitForSeconds(cameraBlendTime);
            cameraController.GetPreviousVC().gameObject.SetActive(false);
        }
    }
}
