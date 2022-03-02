using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseGameEventOnTriggerEnterPlayer : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;
    [SerializeField] private bool onlyOnce = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            gameEvent.Raise();
            if (onlyOnce)
            {
                Destroy(gameObject);
            }
        }
    }
}
