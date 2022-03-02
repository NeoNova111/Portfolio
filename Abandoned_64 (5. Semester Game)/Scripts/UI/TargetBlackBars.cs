using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetBlackBars : MonoBehaviour
{
    [SerializeField] private RectTransform[] barsTransform;
    [SerializeField] private float barTime = 0.2f;

    private float yScale;
    private bool growing;
    public bool Growing { set => growing = value; }

    private void Update()
    {
        if(growing && yScale < 1)
        {
            yScale = Mathf.Clamp01(yScale + Time.deltaTime / barTime);
            foreach (RectTransform rect in barsTransform)
            {
                rect.localScale = new Vector3(1, yScale, 1);
            }
        }
        else if(!growing && yScale >= 0)
        {
            yScale = Mathf.Clamp01(yScale - Time.deltaTime / barTime);
            foreach (RectTransform rect in barsTransform)
            {
                rect.localScale = new Vector3(1, yScale, 1);
            }

            if(yScale == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        growing = true;
        yScale = 0;
        foreach(RectTransform rect in barsTransform)
        {
            rect.localScale = new Vector3(1, yScale, 1);
        }
    }
}