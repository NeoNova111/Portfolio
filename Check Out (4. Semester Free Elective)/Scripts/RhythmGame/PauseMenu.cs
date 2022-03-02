using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject timer;

    private void Start()
    {
        timer.SetActive(false);
    }

    public void Quit()
    {
        GameManager manager = GameManager.instance;
        GameManager.instance.menu = false;
        Time.timeScale = 1;
        manager.LoadLevelSelect();
    }

    public void Resume()
    {
        GameManager.instance.SwitchPauseMenu();
    }

    private void OnEnable()
    {
        RhythmGameManager.instance.musicSource.Pause();
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        RhythmGameManager.instance.musicSource.UnPause();
        Time.timeScale = 1;
    }
}
