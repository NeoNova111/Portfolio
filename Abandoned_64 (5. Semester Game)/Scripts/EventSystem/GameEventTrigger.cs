using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerCondition { OnCall, OnAwake, OnStart, OnEnable, OnDisable }

public class GameEventTrigger : MonoBehaviour
{
    [SerializeField] private GameEvent[] eventsToTrigger;
    [SerializeField] private TriggerCondition trigger;

    private void Awake()
    {
        if (eventsToTrigger.Length == 0)
        {
            enabled = false;
            return;
        }

        if (trigger == TriggerCondition.OnAwake) RaiseEvents();
    }

    void Start()
    {
        if (trigger == TriggerCondition.OnStart) RaiseEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RaiseEvents()
    {
        foreach(GameEvent gameEvent in eventsToTrigger)
        {
            gameEvent.Raise();
        }
    }

    private void OnEnable()
    {
        if (trigger == TriggerCondition.OnEnable) RaiseEvents();
    }

    private void OnDisable()
    {
        if (trigger == TriggerCondition.OnDisable) RaiseEvents();
    }
}
