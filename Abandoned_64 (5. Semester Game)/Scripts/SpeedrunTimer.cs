using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedrunTimer : MonoBehaviour
{
    #region Singeton

    private static SpeedrunTimer instance;

    public static SpeedrunTimer Instance { get => instance; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    private TimeSpan timePlaying;
    public Text timeCounter;
    private bool timerGoing = false;
    private float elapsedTime;
    private bool timerHasAlreadyBegun = false;

    // Start is called before the first frame update
    /*void Start()
    {
        timeCounter.text = "00:00.00";
        timerGoing = false;
    }*/
    

    public void BeginTimer()
    {
        if (!timerHasAlreadyBegun)
        {
        timeCounter.text = "00:00.00";
        timerGoing = true;
        elapsedTime = 0f;
        timerHasAlreadyBegun = true;
        StartCoroutine(UpdateTimer());
        }
    }

    public void PauseTimer()
    {
        timerGoing = false;
    }

    public void unPauseTimer()
    {
        if (timerHasAlreadyBegun)
        {
        timerGoing = true;
        StartCoroutine(UpdateTimer());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void EndTimer()
    {
        timerGoing = false;
    }

    private IEnumerator UpdateTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timePlayingStr = timePlaying.ToString("mm':'ss'.'ff"); // "Time: " + 
            timeCounter.text = timePlayingStr;

            yield return null;
        }
    }
}
