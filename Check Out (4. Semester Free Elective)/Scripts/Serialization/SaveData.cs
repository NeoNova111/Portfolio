using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<CharacterSaveData> characterSaveData;
    public bool playedBefore = false;
    public bool wonBefore = false;

    public SaveData()
    {
        characterSaveData = new List<CharacterSaveData>();
        playedBefore = false;
        wonBefore = false;
    }
}

[System.Serializable]
public struct CharacterSaveData
{
    public string characterName;
    public int characterHighestAffection;
    public int characterSongHighScore;
    public int characterSongHighestCombo;

    public CharacterSaveData(string name, int affection, int score, int combo)
    {
        characterName = name;
        characterHighestAffection = affection;
        characterSongHighScore = score;
        characterSongHighestCombo = combo;
    }
}
