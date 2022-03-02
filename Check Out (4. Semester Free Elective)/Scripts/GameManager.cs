using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null)
        {
            Debug.LogWarning("More than one GameManager instance: destroying new one");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion

    public Song currentSong;
    public DialogueCharacter currentCharacter;
    public DialogueCharacter[] allCharacters;
    public bool lastGameSuccess = false;
    public bool cameFromGame = false;
    public bool wonBefore = false;
    public GameEvent alreadyWon;

    [SerializeField] private GameEvent enterMenu;
    [SerializeField] private GameEvent exitMenu;
    public bool menu = false;

    [SerializeField] private SceneNames sceneNames;

    [System.Serializable]
    struct SceneNames
    {
        public string mainMenu;
        public string levelSelect;
        public string rhythmGame;
    }


    private void Start()
    {
        SaveManager.instance.ApplyLoadedGameData();
        if (wonBefore)
        {
            alreadyWon.Raise();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchPauseMenu();
        }
    }

    public void SwitchPauseMenu()
    {
        if (menu)
        {
            exitMenu.Raise();
        }
        else
        {
            enterMenu.Raise();
        }

        menu = !menu;
    }

    #region loading scenes

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(sceneNames.mainMenu);
    }

    public void DelayLoadMainMenu(float t)
    {
        LoadSceneSeconds(t, sceneNames.mainMenu);
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene(sceneNames.levelSelect);
    }

    public void DelayLoadLevelSelect(float t)
    {
        LoadSceneSeconds(t, sceneNames.levelSelect);
    }

    public void LoadRhythmGame()
    {
        SceneManager.LoadScene(sceneNames.rhythmGame);
    }

    public void LoadSceneSeconds(float time, int i)
    {
        StartCoroutine(LoadSceneAfterSeconds(time, i));
    }

    public void LoadSceneSeconds(float time, string s)
    {
        StartCoroutine(LoadSceneAfterSeconds(time, s));
    }

    public void TestingLoad()
    {
        LoadSceneSeconds(5, 1);
    }

    IEnumerator LoadSceneAfterSeconds(float time, int i)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(i);
    }

    IEnumerator LoadSceneAfterSeconds(float time, string s)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(s);
    }

    #endregion

    public void StartLevel()
    {
        currentCharacter.tempAffection = 0;
        currentCharacter.AddAffection(0);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnNewScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnNewScene;
    }

    void OnNewScene(Scene scene, LoadSceneMode mode)
    {
        if (menu)
        {
            SwitchPauseMenu();
        }

        Cursor.visible = true;
    }
}
