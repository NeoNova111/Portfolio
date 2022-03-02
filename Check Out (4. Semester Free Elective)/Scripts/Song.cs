using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Song")]
public class Song : ScriptableObject
{
    public int highScore = 0;
    public int highestCombo = 0;
    public int bpm = 0;
    public float secondsTillLine = 1.5f;
    public AudioClip music;
    [Range(0,1)] public float targetScorePercentage = 0.5f;
    public TimingChanger changeTiming;
    public List<NoteSpawn> notesToSpawn;
}

[System.Serializable]
public struct NoteSpawn
{
    public NoteType typeToSpawn;
    public float noteLength;
    public Sprite overrideSprite;
    public int lineToSpawnOn;
    public float timingInSeconds;

    public NoteSpawn(NoteType type, int line, float timing)
    {
        typeToSpawn = type;
        overrideSprite = null;
        lineToSpawnOn = line;
        timingInSeconds = timing;
        noteLength = 0;
    }

    public NoteSpawn(NoteType type, Sprite overSprite, int line, float timing)
    {
        typeToSpawn = type;
        overrideSprite = overSprite;
        lineToSpawnOn = line;
        timingInSeconds = timing;
        noteLength = 0;
    }

    public NoteSpawn(NoteType type, Sprite overSprite, int line, float timing, float length)
    {
        typeToSpawn = type;
        overrideSprite = overSprite;
        lineToSpawnOn = line;
        timingInSeconds = timing;
        noteLength = 0;
        noteLength = length;
    }

    public NoteSpawn(NoteType type, int line, float timing, float length)
    {
        typeToSpawn = type;
        overrideSprite = null;
        lineToSpawnOn = line;
        timingInSeconds = timing;
        noteLength = 0;
        noteLength = length;
    }
}

[System.Serializable]
public struct TimingChanger
{
    public bool change;
    public Vector2 indexRange;
    public float changeBySec;
}
