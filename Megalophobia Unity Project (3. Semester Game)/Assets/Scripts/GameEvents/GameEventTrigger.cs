using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventTrigger : MonoBehaviour
{
    public GameEvent[] events;
    public int triggerAmount = 1;
    public float triggerDelay = 5;
    float delay;

    private void Start()
    {
        delay = 0;
    }

    private void Update()
    {
        if (delay > 0)
            delay -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && triggerAmount > 0 && delay <= 0)
        {
            TriggerEvents();
        }
    }

    void TriggerEvents()
    {
        triggerAmount--;
        delay = triggerDelay;
        foreach (GameEvent gameEvent in events)
        {
            if (gameEvent)
                gameEvent.Raise();
            else
                Debug.LogWarning("Cant Trigger Event that's not added");
        }
    }
}
