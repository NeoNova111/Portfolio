using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    #region Singelton

    public static SaveManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null)
        {
            Debug.LogWarning("More than one SaveManager instance");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion

    public SaveData gameSaveData;

    private void Start()
    {
        LoadGameData();
        Debug.Log(Application.persistentDataPath);
    }

    public void SaveGameData()
    {
        gameSaveData = new SaveData();

        if (GameManager.instance)
        {
            GameManager gi = GameManager.instance;
            bool properAffectionLevel = true;
            foreach (DialogueCharacter diaChar in gi.allCharacters)
            {
                gameSaveData.characterSaveData.Add(new CharacterSaveData(diaChar.characterName, diaChar.affectionLevel, diaChar.rhythmGameSong.highScore, diaChar.rhythmGameSong.highestCombo));
                if (diaChar.affectionLevel < 2)
                    properAffectionLevel = false;
            }
            gameSaveData.wonBefore = properAffectionLevel;
        }
        else
            Debug.LogError("no gameManager instance to save from");

        if (!gameSaveData.playedBefore)
            gameSaveData.playedBefore = true;

        string json = JsonUtility.ToJson(gameSaveData);
        File.WriteAllText(Application.persistentDataPath + "/saveData.json", json);
    }

    public void LoadGameData()
    {
        if (!SaveDataExists())
        {
            Debug.LogWarning("no save file to load from");
            return;
        }

        string json = File.ReadAllText(Application.persistentDataPath + "/saveData.json");
        gameSaveData = JsonUtility.FromJson<SaveData>(json);
    }

    public void ApplyLoadedGameData()
    {
        //LoadGameData();
        if (GameManager.instance)
        {
            GameManager gi = GameManager.instance;
            foreach (DialogueCharacter diaChar in gi.allCharacters)
            {
                foreach(CharacterSaveData charData in gameSaveData.characterSaveData)
                {
                    if(charData.characterName == diaChar.characterName)
                    {
                        diaChar.affectionLevel = charData.characterHighestAffection;
                        diaChar.rhythmGameSong.highestCombo = charData.characterSongHighestCombo;
                        diaChar.rhythmGameSong.highScore = charData.characterSongHighScore;
                        break;
                    }
                }
            }
            gi.wonBefore = gameSaveData.wonBefore;
        }
        else
            Debug.LogWarning("no gameManager instance or saved data");
    }

    public bool SaveDataExists()
    {
        return File.Exists(Application.persistentDataPath + "/saveData.json");
    }
}
