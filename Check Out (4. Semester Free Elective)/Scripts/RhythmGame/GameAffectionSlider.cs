using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameAffectionSlider : MonoBehaviour
{
    [SerializeField] Slider progressSlider;
    [SerializeField] Slider targetSlider;
    public GameEvent reachedAffectionTarget;
    bool triggeredOnce = false;

    public bool TargetReached()
    {
        if(progressSlider.value >= targetSlider.value)
        {
            if (!triggeredOnce)
            {
                reachedAffectionTarget.Raise();
                triggeredOnce = true;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Setup(float targetPercentage)
    {
        progressSlider.value = 0;
        targetSlider.value = targetPercentage;
    }

    public void UpdateValue(int score, int potential)
    {
        progressSlider.value = (float)score / (float)potential;
    }
}
