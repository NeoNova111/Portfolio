using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnterTrigger : MonoBehaviour
{
    [SerializeField] private MainRoom room;

    private void Awake()
    {
        if (!room) room = transform.root.GetComponent<MainRoom>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>()) room.EnterRoom();
    }
}
