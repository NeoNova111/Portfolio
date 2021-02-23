using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    //change to work with singleton instances in the future
    //public SubStats submarineStats;
    //public SoundSettings soundSettings;

    public PlayerSaveData loadData;
    public SettingsSaveData loadSettings;
    public bool loadFromSave;

    private void Start()
    {
        LoadPlayerData();
    }

    public void SavePlayerData()
    {
        PlayerSaveData saveData = new PlayerSaveData();

        saveData.levelIdx = SceneManager.GetActiveScene().buildIndex;

        if (submarineMovement.instance)
        {
            submarineMovement subInstance = submarineMovement.instance;
            saveData.savePosition = subInstance.submarineStats.submarinePosition;
            saveData.playerHealth = subInstance.submarineStats.health;
        }
        else
            Debug.LogWarning("can't save player in current state");

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/saveData.json", json);
    }

    public void LoadPlayerData()
    {
        if(!PlayerSaveFileExists())
        {
            Debug.LogWarning("save file does not exist");
            return;
        }

        string json = File.ReadAllText(Application.persistentDataPath + "/saveData.json");
        PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);

        loadData = saveData;
    }

    public void SaveSettingsData()
    {
        SettingsSaveData saveData = new SettingsSaveData();

        if (AudioManager.instance)
        {
            AudioManager managerInstance = AudioManager.instance;
            saveData.masterVolume = managerInstance.currentSoundSettings.masterVolume;
            saveData.musicVolume = managerInstance.currentSoundSettings.musicVolume;
            saveData.sfxVolume = managerInstance.currentSoundSettings.sfxVolume;
            saveData.voiceVolume = managerInstance.currentSoundSettings.voiceoverVolume;
        }
        else
            Debug.LogWarning("can't save settings in current state");

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/settingsSaveData.json", json);
    }

    public void LoadSettingsData()
    {
        if(!SettingsSaveFileExists())
        {
            Debug.LogWarning("save file does not exist");
            return;
        }

        string json = File.ReadAllText(Application.persistentDataPath + "/settingsSaveData.json");
        SettingsSaveData saveData = JsonUtility.FromJson<SettingsSaveData>(json);

        loadSettings = saveData;
    }

    public bool PlayerSaveFileExists()
    {
        return File.Exists(Application.persistentDataPath + "/saveData.json");
    }

    public bool SettingsSaveFileExists()
    {
        return File.Exists(Application.persistentDataPath + "/settingsSaveData.json");
    }
}

public class PlayerSaveData
{
    public int levelIdx;

    public Vector3 savePosition;
    public float playerHealth;
}

public class SettingsSaveData
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float voiceVolume;
}
