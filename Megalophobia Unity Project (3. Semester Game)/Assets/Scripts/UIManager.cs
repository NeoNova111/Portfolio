using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Singelton

    public static UIManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null)
        {
            Debug.LogWarning("More than one UIManager instance");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    #endregion

    public AbilityUI abilityUI;
    public GameEvent pauseGame;
    public GameEvent unpauseGame;
    bool paused = false;

    private void Start()
    {
        Unpause();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (paused)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
    }

    public void Unpause()
    {
        unpauseGame.Raise();
        paused = false;
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        pauseGame.Raise();
        paused = true;
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}
