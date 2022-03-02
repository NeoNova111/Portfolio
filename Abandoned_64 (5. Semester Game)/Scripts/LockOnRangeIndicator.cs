using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnRangeIndicator : MonoBehaviour
{
    CameraController camController;
    PlayerStateMachine playerStateMachine;
    MeshRenderer renderer;
    Transform playerTransform;
    float lockOnDistance;
    

    // Start is called before the first frame update
    void Start()
    {
        camController = CameraController.Instance;
        playerTransform = camController.transform;
        playerStateMachine = PlayerStateMachine.Instance;
        renderer = gameObject.GetComponent<MeshRenderer>();
        lockOnDistance = camController.LockOnDistance;
        //gameObject.transform.localScale = new Vector3(1, 1, 1) * camController.LockOnDistance / 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (camController.LockOnTarget == null || camController.LockOnTarget.Equals(null))
        {
            renderer.enabled = false;
        }
        else
        {
            renderer.enabled = true;
            Vector3 planePos = camController.LockOnTarget.TargetTransform.position;
            planePos.y = playerTransform.position.y;
            gameObject.transform.position = planePos;
            float yDistanceEnemyPlayer = Mathf.Abs(playerTransform.position.y - camController.LockOnTarget.TargetTransform.position.y);
            float newScale = Mathf.Sqrt(lockOnDistance * lockOnDistance - yDistanceEnemyPlayer * yDistanceEnemyPlayer);
            if (!float.IsNaN(newScale))
            {
                gameObject.transform.localScale = new Vector3(1, 1, 1) * newScale / 5;
            }
        }
        
    }
}
