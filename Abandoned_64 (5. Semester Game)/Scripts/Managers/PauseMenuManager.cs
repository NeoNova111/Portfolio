using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    private Stack<GameObject> menuStack = new Stack<GameObject>();
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameEvent MainMenuOpens;
    [SerializeField] private GameEvent MainMenuCloses;
    public bool MenuIsActive { get { return menuStack.Count > 0; }  }

    private void Awake()
    {
        menuStack = new Stack<GameObject>();
    }

    private void Update()
    {
        if (PlayerStateMachine.Instance.WasMenuPressedThisFrame)
        {
            if (menuStack.Count == 0)
            {
                MainMenuOpens.Raise();
                OpenMenu(mainMenu);
            }
            else
            {
                MainMenuCloses.Raise();
                CloseMenu();
            }
        }
    }

    public void CloseApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OpenMenu(GameObject gameObject)
    {
        if(menuStack.Count != 0)
        {
            menuStack.Peek().SetActive(false);
            MainMenuOpens.Raise();
        }
        else
        {
            //Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
        }

        menuStack.Push(gameObject);
        menuStack.Peek().SetActive(true);
    }

    public void CloseMenu()
    {
        menuStack.Pop().SetActive(false);

        if (menuStack.Count != 0)
        {
            menuStack.Peek().SetActive(true);
        }
        else if(DialogueSystem.Instance.IsWaitingForChoice) //todo: make less clunky 
        {
            DialogueSystem.Instance.choiceOne.transform.parent.GetComponent<Selectable>().Select();
            Time.timeScale = 1;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }
}
