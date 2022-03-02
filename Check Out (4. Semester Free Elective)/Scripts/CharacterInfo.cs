using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    public GameObject[] hearts;
    public Text highScore;
    public Text combo;
    GameManager gmi;

    public void AdaptToCurrentCharacter()
    {
        gmi = GameManager.instance;
        if(gmi && gmi.currentCharacter)
        {
            foreach(GameObject g in hearts)
            {
                g.SetActive(false);
            }

            for(int i = 0; i < gmi.currentCharacter.affectionLevel; i++)
            {
                hearts[i].SetActive(true);
            }

            highScore.text = "" + gmi.currentSong.highScore;
            combo.text = "" + gmi.currentSong.highestCombo;
        }
    }

    private void OnEnable()
    {
        AdaptToCurrentCharacter();
    }
}
