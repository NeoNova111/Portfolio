using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SubStats stats;
    public GameObject continueButton;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        if (SaveManager.instance.PlayerSaveFileExists())
            continueButton.SetActive(true);
        else
            continueButton.SetActive(false);
    }

    public void NewGame()
    {
        stats.health = stats.maxHealth;
        SaveManager.instance.loadFromSave = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ContinueGame()
    {
        stats.health = SaveManager.instance.loadData.playerHealth;
        SaveManager.instance.loadFromSave = true;
        SceneManager.LoadScene(SaveManager.instance.loadData.levelIdx);
    }

    public void SelectChapter(int idx)
    {
        stats.health = stats.maxHealth;
        SaveManager.instance.loadFromSave = false;

        if (idx >= SceneManager.sceneCountInBuildSettings)
            idx = SceneManager.sceneCountInBuildSettings - 1;
        else if (idx <= 0)
            idx = 1;

        SceneManager.LoadScene(idx);
    }

    public void OpenMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
