using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TempBuffTimer : MonoBehaviour
{
    public float RemainingTime { set => remainingTime = value; }

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image bgFlash;
    [SerializeField] private Image buffIcon;
    private float remainingTime;
    private float timeSinceInit;

    void Update()
    {
        if (timeSinceInit < 0.1f)
        {
            timeSinceInit = Mathf.Clamp(timeSinceInit + Time.deltaTime, 0, 0.1f);
            float alphaValue = 1f - timeSinceInit / 0.1f;
            Color newColor = bgFlash.color;
            newColor.a = alphaValue;
            bgFlash.color = newColor;
        }

        remainingTime = Mathf.Clamp(remainingTime - Time.deltaTime, 0, remainingTime);
        var ts = TimeSpan.FromSeconds(remainingTime);
        text.text = ts.ToString(@"mm\:ss\:fff");

        if(remainingTime <= 1f)
        {
            float alphaValue = 1f - remainingTime / 1f;
            Color newColor = bgFlash.color;
            newColor.a = alphaValue;
            bgFlash.color = newColor;
        }
    }

    public void InitRemainingTime(float time, Sprite s)
    {
        remainingTime = time;
        timeSinceInit = 0;
        buffIcon.sprite = s;
    }
}
