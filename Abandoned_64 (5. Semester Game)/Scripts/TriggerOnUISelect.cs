using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TriggerOnUISelect : MonoBehaviour, ISelectHandler
{
    [SerializeField] private GameEvent[] selectEvents;

    public void OnSelect(BaseEventData eventData)
    {
        foreach (GameEvent e in selectEvents)
        {
            e.Raise();
        }
    }
}
