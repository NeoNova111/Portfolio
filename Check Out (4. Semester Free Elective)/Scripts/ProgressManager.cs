using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    // Start is called before the first frame update
    GameManager gameManager;

    public Animator picAnimator;
    public AudioSource shutter;

    void Start()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FinalAffectionCheck()
    {
        DialogueSystem dialogueSystem = DialogueSystem.instance;
        Debug.Log("checking affection");

        dialogueSystem.StopSpeaking();

        if (gameManager.currentCharacter.tempAffection >=2)
        {
            dialogueSystem.SetConvo(gameManager.currentCharacter.finalSuccess);
        }
        else
        {
            dialogueSystem.SetConvo(gameManager.currentCharacter.finalFail);
        }

        dialogueSystem.SayCurentLine();
        FinishLevel();
    }

    void FinishLevel()
    {
        if(gameManager.currentCharacter.tempAffection > gameManager.currentCharacter.affectionLevel)
        {
            gameManager.currentCharacter.SetAffection(gameManager.currentCharacter.tempAffection);
        }    

        SaveManager.instance.SaveGameData();
    }

    public void CheckIfWon()
    {
        if (GameManager.instance.wonBefore)
            return;

        foreach(DialogueCharacter chara in GameManager.instance.allCharacters)
        {
            if(chara.affectionLevel < 2)
            {
                return;
            }
        }

        picAnimator.SetTrigger("TakePic");
        shutter.Play();
        gameManager.wonBefore = true;
    }
}
