using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ApplicationScript : MonoBehaviour
{
    #region Singelton
    public static ApplicationScript instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null)
        {
            Debug.LogWarning("More than one applicationScript instance");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    #endregion

    public GameEvent menuEnter;
    public GameEvent menuExit;

    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject optionMenu;
    private GameObject activeMenu;

    public GameObject pauseButton;
    public GameObject playButton;

    public enum MenuState {START, PAUSE, DEACTIVATED}
    public MenuState menuState;

    public Texture2D mouseCursor;

    private void Start()
    {
        MenuSetup();
        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown("escape"))
        {
            if (menuState == MenuState.PAUSE)
            {
                ContinueGame();
            }
            else if (menuState == MenuState.DEACTIVATED)
            {
                PauseGame();
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #region Menu Fumctions

    public void MenuSetup()
    {
        menuEnter.Raise();
        menuState = MenuState.START;
        optionMenu.SetActive(false);
        startMenu.SetActive(true);
        pauseMenu.SetActive(false);
        playButton.SetActive(false);
        pauseButton.SetActive(false);
        activeMenu = startMenu;
        Time.timeScale = 0;

    }

    public void ContinueGame()
    {
        menuExit.Raise();
        Time.timeScale = 1;
        activeMenu.SetActive(false);
        pauseButton.SetActive(true);
        playButton.SetActive(false);
        menuState = MenuState.DEACTIVATED;
    }

    public void PauseGame()
    {
        menuEnter.Raise();
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        playButton.SetActive(true);
        activeMenu = pauseMenu;
        menuState = MenuState.PAUSE;
    }

    public void OpenOptionsMenu()
    {
        optionMenu.SetActive(true);
        activeMenu.SetActive(false);
        activeMenu = optionMenu;
    }

    public void Back()
    {
        switch (menuState)
        {
            case MenuState.START:
                startMenu.SetActive(true);
                activeMenu.SetActive(false);
                activeMenu = startMenu;
                break;
            case MenuState.PAUSE:
                pauseMenu.SetActive(true);
                activeMenu.SetActive(false);
                activeMenu = pauseMenu;
                break;
                
        }
    }

    public void StartGame()
    {
        ContinueGame();
    }

    #endregion
}
