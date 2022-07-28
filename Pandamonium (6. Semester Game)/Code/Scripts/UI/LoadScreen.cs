using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    public float timeStep = 0.1f;
    public Image image;

    private IEnumerator spinny;

    private void OnEnable()
    {
        spinny = SpinnyRoutine();
        StartCoroutine(spinny);
    }

    private void OnDisable()
    {
        StopCoroutine(spinny);
    }

    IEnumerator SpinnyRoutine()
    {
        float passedtime = 0f;
        WaitForSecondsRealtime step = new WaitForSecondsRealtime(timeStep);
        while (true)
        {
            image.fillAmount = passedtime;
            yield return step;
            passedtime += timeStep;
            if (passedtime > 1f) passedtime -= 1f;
        }
    }
}
