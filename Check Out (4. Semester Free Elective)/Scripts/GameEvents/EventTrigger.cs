using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public GameEvent[] events;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
            TriggerEvents();
    }

    public void TriggerEvents()
    {
        foreach (GameEvent evt in events)
            evt.Raise();
    }
}

