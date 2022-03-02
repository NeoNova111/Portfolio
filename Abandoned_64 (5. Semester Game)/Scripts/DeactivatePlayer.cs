using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivatePlayer : MonoBehaviour
{
    private GameObject playerMesh = null;

    public void deactivatePlayer()
    {
        if(playerMesh == null)
        {
            playerMesh = PlayerStateMachine.Instance.transform.GetChild(0).gameObject;
        }
        
        playerMesh.SetActive(false);
    }
    public void activatePlayer()
    {
        if (playerMesh == null)
        {
            playerMesh = PlayerStateMachine.Instance.transform.GetChild(0).gameObject;
        }

        playerMesh.SetActive(true);
    }
}
