using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseGameEvent : MonoBehaviour
{
    [SerializeField] private GameEvent raiseEvent;
    void Start()
    {
        raiseEvent.Raise();
    }
}
