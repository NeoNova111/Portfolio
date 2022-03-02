using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject ProfileScreen;
    public GameObject OptionsScreen;
    public GameObject speedrunTimer;
    public Toggle speedrunToggle;

    public Button[] HighlightButtons;

    private SaveManager saveManager;

    public void Start()
    {
        if (MainMenu != null)
        {
            ReturnToMain();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        saveManager = SaveManager.Instance;
    }

    public void PickProfile()
    {
        MainMenu.SetActive(false);
        ProfileScreen.SetActive(true);
        Refocus();
    }
    public void ReturnToMain()
    {
        MainMenu.SetActive(true);
        ProfileScreen.SetActive(false);
        OptionsScreen.SetActive(false);
        Refocus();
    }

    public void OpenOptions()
    {
        OptionsScreen.SetActive(true);
        MainMenu.SetActive(false);
        ProfileScreen.SetActive(false);
        Refocus();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void RunCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void WipeSaveFiles()
    {
        saveManager.ResetSaveData();
    }

    public void StartGame()
    {
        PlayerSaveData playerSaveData = saveManager.PlayerLoadData;
        if (playerSaveData != null && saveManager.PlayerSaveFileExists())
        {
            SceneManager.LoadScene(playerSaveData.levelIdx);
        }
        else
        {
            SceneManager.LoadScene("IntroScene");
            //SceneManager.LoadScene("Level1"); //for faster load testing
        }
    }

    public void GoToMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }
   
    public void Refocus()
    {
        foreach(Button b in HighlightButtons)
        {
            if (b.gameObject.activeInHierarchy)
            {
                b.Select();
            }
        }
    }

    public void TrueTimerToggle()
    {
        if (speedrunTimer.activeSelf)
        {
            speedrunTimer.SetActive(false);
            speedrunToggle.isOn = false;
        }
        else
        {
            speedrunTimer.SetActive(true);
            speedrunToggle.isOn = true;
        }
    }
}


