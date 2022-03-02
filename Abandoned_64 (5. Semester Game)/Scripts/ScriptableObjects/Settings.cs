using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings Object")]
public class Settings : ScriptableObject
{
    public GameEvent settingsChanged;
    public Vector2 keyboardSensitivity;
    public Vector2 controllerSensitivity;

    public void OnValidate()
    {
        settingsChanged.Raise();
    }
}
