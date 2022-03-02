using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [System.SerializableAttribute]
    public struct TimerText
    {
        public string text;
        public int size;
        public Color color;
    }

    public TextMeshProUGUI counterText;
    public GameObject pauseMenu;
    public TimerText[] timerText;
    private IEnumerator coroutine;

    private void OnEnable()
    {
        coroutine = Counter();
        StartCoroutine(coroutine);
    }

    private void OnDisable()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    IEnumerator Counter()
    {
        foreach(TimerText t in timerText)
        {
            counterText.text = t.text;
            counterText.color = t.color;
            counterText.fontSize = t.size;
            yield return new WaitForSecondsRealtime(1f);
        }

        pauseMenu.SetActive(false);
    }
}
