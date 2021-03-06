﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Line
{
    public DialogueCharacter character;

    [TextArea(2, 5)]
    public string text;
    public bool hasVoceOver;
    public SoundClip voiceLine;
}

[CreateAssetMenu(menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public bool triggersNextDialogue = false;
    public DialogueCharacter leftSpeaker;
    public DialogueCharacter rightSpeaker;
    public Line[] lines;

    int activeLineIdx = 0;

    public int GetActiveLineIdx()
    {
        return activeLineIdx;
    }

    public void SetActiveLineIdx(int idx)
    {
        activeLineIdx = idx;
    }
}
