using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsScreen : MonoBehaviour
{
    public GameObject fail;
    public GameObject success;

    private void Start()
    {
        fail.SetActive(false);
        success.SetActive(false);
    }

    public void Evaluate()
    {
        if (RhythmGameManager.instance.WonAffection())
        {
            success.SetActive(true);
            if(GameManager.instance.currentCharacter)
                GameManager.instance.currentCharacter.AddAffection(1);
        }
        else
        {
            fail.SetActive(true);
        }
    }
}
