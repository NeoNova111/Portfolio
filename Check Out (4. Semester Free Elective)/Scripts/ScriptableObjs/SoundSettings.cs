using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SoundSettings")]
public class SoundSettings : ScriptableObject
{
    [Range (0, 1)]
    [Tooltip ("Master volume in percentage")]
    public float masterVolume = 1f;

    [Range (0, 1)]
    [Tooltip("Music volume in percentage")]
    public float musicVolume = 1f;

    [Range (0, 1)]
    [Tooltip("Sfx volume in percentage")]
    public float sfxVolume = 1f;

    [Range (0, 1)]
    [Tooltip("Voiceover volume in percentage")]
    public float voiceoverVolume = 1f;
}
