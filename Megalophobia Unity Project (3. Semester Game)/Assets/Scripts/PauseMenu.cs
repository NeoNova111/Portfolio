using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject defaultMenu;
    GameObject currentActiveMenu;

    public GameEvent buttonClicked;

    private void Awake()
    {
        defaultMenu.SetActive(true);
        currentActiveMenu = defaultMenu;
    }

    public void ChangeMenu(GameObject newMenu)
    {
        newMenu.SetActive(true);
        currentActiveMenu.SetActive(false);
        currentActiveMenu = newMenu;
    }

    public void Continue()
    {
        UIManager.instance.Unpause();
    }

    public void ChangeToDefaultMenu()
    {
        currentActiveMenu.SetActive(false);
        currentActiveMenu = defaultMenu;
        defaultMenu.SetActive(true);
    }

    public void ButtonClicked()
    {
        buttonClicked.Raise();
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
