using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    #region Singelton

    private static SaveManager instance;
    public static SaveManager Instance { get => instance; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Setup();
    }

    #endregion

    private PlayerSaveData playerData;
    private string playerFileName = "/saveData.json";
    private SettingsSaveData settingsData;
    private string settingsFileName = "/settingsSaveData.json";
    private SceneSaveData sceneData;
    private string sceneFileName = "/sceneSaveData.json";
    [SerializeField] private Settings customSettings;
    [SerializeField] private SoundSettings customSoundSettings;

    public PlayerSaveData PlayerLoadData { get => playerData; }
    public SettingsSaveData SettingsLoadData { get => settingsData; }
    public SceneSaveData SceneLoadData { get => sceneData; }

    private void Setup()
    {   
        LoadPlayerData();
        LoadSettingsData();
        LoadSceneData();
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M))
        {
            SavePlayerData();
        }
        #endif
    }

    public void SavePlayerData()
    {
        if (PlayerStateMachine.Instance)
        {
            PlayerStateMachine player = PlayerStateMachine.Instance;
            SavePlayerData(player.transform.position, player.transform.eulerAngles);
        }
    }
 
    public void SavePlayerData(Vector3 positionOverride, Vector3 rotationOverride)
    {
        PlayerSaveData saveData = new PlayerSaveData();

        saveData.levelIdx = SceneManager.GetActiveScene().buildIndex;

        if (PlayerStateMachine.Instance)
        {
            PlayerStateMachine player = PlayerStateMachine.Instance;
            saveData.savePosition = positionOverride;
            saveData.saveRotation = Quaternion.Euler(rotationOverride);
            saveData.playerMaxHealth = player.PlayerStats.maxHealth;
            saveData.playerCurrentHealth = player.PlayerStats.CurrentHealth;
            saveData.collectibleCount = player.PlayerStats.collectibleCount;
            saveData.keyItemCount = player.PlayerStats.keyCount;
            saveData.allowedToUseDebug = player.PlayerStats.allowedToUseDebug; //set this manually later on

            player.PlayerStats.respawning = true;
        }
        else
            Debug.LogWarning("can't save player in current state");

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + playerFileName, json);
    }

    public void ResetSaveData()
    {
        if (PlayerSaveFileExists())
        {
            File.Delete(Application.persistentDataPath + playerFileName);
        }

        if (SceneSaveFileExists())
        {
            File.Delete(Application.persistentDataPath + sceneFileName);
        }
    }

    public void LoadPlayerData()
    {
        if (!PlayerSaveFileExists())
        {
            Debug.LogWarning("save file does not exist, applying default settings");
            return;
        }

        string json = File.ReadAllText(Application.persistentDataPath + playerFileName);
        PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);

        playerData = saveData;
    }

    public void SaveSettingsData()
    {
        SettingsSaveData saveData = new SettingsSaveData();

        if (customSettings)
        {
            saveData.mkHorizontalSens = customSettings.keyboardSensitivity.x;
            saveData.mkVerticalSens = customSettings.keyboardSensitivity.y;
            saveData.contollerHorizontalSens = customSettings.controllerSensitivity.x;
            saveData.controllerVertivalSens = customSettings.controllerSensitivity.y;

            saveData.masterVolume = customSoundSettings.masterVolume;
            saveData.sfxVolume = customSoundSettings.sfxVolume;
            saveData.musicVolume = customSoundSettings.musicVolume;
        }
        else
            Debug.LogWarning("missing Settings Asset in SaveManager");

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + settingsFileName, json);
    }

    public void LoadSettingsData()
    {
        if (!SettingsSaveFileExists())
        {
            Debug.LogWarning("settings save file does not exist, applying default settings");

            customSettings.keyboardSensitivity.x = 0.1f;
            customSettings.keyboardSensitivity.y = 0.001f;
            customSettings.controllerSensitivity.x = 300f;
            customSettings.controllerSensitivity.y = 2f;

            customSoundSettings.masterVolume = 1;
            customSoundSettings.musicVolume = 1;
            customSoundSettings.sfxVolume = 1;

            SaveSettingsData();
            return;
        }

        string json = File.ReadAllText(Application.persistentDataPath + settingsFileName);
        SettingsSaveData saveData = JsonUtility.FromJson<SettingsSaveData>(json);

        settingsData = saveData;
        customSettings.keyboardSensitivity.x = settingsData.mkHorizontalSens;
        customSettings.keyboardSensitivity.y = settingsData.mkVerticalSens;
        customSettings.controllerSensitivity.x = settingsData.contollerHorizontalSens;
        customSettings.controllerSensitivity.y = settingsData.controllerVertivalSens;

        customSoundSettings.masterVolume = settingsData.masterVolume;
        customSoundSettings.sfxVolume = settingsData.sfxVolume;
        customSoundSettings.musicVolume = settingsData.musicVolume;

        customSettings.settingsChanged.Raise();
    }

    public void SaveSceneData()
    {
        string json = JsonUtility.ToJson(sceneData);
        File.WriteAllText(Application.persistentDataPath + sceneFileName, json);
    }
    
    public void LoadSceneData()
    {
        if (!SceneSaveFileExists())
        {
            Debug.LogWarning("scene save file does not exist");
            sceneData = new SceneSaveData();
            return;
        }

        string json = File.ReadAllText(Application.persistentDataPath + sceneFileName);
        SceneSaveData saveData = JsonUtility.FromJson<SceneSaveData>(json);

        sceneData = saveData;
    }

    public bool PlayerSaveFileExists()
    {
        return File.Exists(Application.persistentDataPath + playerFileName) && File.ReadAllText(Application.persistentDataPath + playerFileName) != "";
    }

    public bool SettingsSaveFileExists()
    {
        return File.Exists(Application.persistentDataPath + settingsFileName) && File.ReadAllText(Application.persistentDataPath + settingsFileName) != "";
    }

    public bool SceneSaveFileExists()
    {
        return File.Exists(Application.persistentDataPath + sceneFileName) && File.ReadAllText(Application.persistentDataPath + sceneFileName) != "";
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    //save player data when new scene is loaded 
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SavePlayerData();
        Debug.Log("saved");
    }

    void OnSceneUnloaded(Scene scene)
    {
        SaveSceneData();
    }
}

//all the save file types
public class PlayerSaveData
{
    public int levelIdx = 0;
    public bool allowedToUseDebug;

    public Vector3 savePosition;
    public Quaternion saveRotation;

    //playerStats
    public int playerMaxHealth;
    public float playerCurrentHealth;
    public int collectibleCount;
    public int keyItemCount;
}

public class SettingsSaveData
{
    public float mkHorizontalSens;
    public float mkVerticalSens;
    public float contollerHorizontalSens;
    public float controllerVertivalSens;

    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;
}

public class SceneSaveData
{
    public SceneSaveData()
    {
        lockInfo = new List<LockInfo>();
    }

    public List<LockInfo> lockInfo;
}

