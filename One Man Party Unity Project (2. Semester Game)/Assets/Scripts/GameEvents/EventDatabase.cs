using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/EventDatabase")]
public class EventDatabase : ScriptableObject
{
    public GameEvent[] events;
}
