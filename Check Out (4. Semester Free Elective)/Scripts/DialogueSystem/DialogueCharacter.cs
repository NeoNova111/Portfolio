using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/ DialogueCharacter")]
public class DialogueCharacter : ScriptableObject
{
    public string characterName;
    [Tooltip ("0:defaul, 1:success, 2:fail")] public Sprite[] characterSprites;
    [Tooltip("products unique to the character")] public Sprite[] specialProductSprites;
    public Sprite chibiSprite;
    [Range(0, 3)] public int affectionLevel;
    [HideInInspector] [Range(0, 3)] public int tempAffection = 0;
    public Song rhythmGameSong;
    public DialogueConversation smallTalk;
    public DialogueConversation gameSuccess;
    public DialogueConversation gameFail;
    public DialogueConversation finalSuccess;
    public DialogueConversation finalFail;
    [SerializeField] GameEvent affectionsChange;

    public void AddAffection(int add)
    {
        add = Mathf.Clamp(add, -1, 1);
        
        if(add == 1)
        {
            tempAffection++;
            if (tempAffection > 3) tempAffection = 3;
        }
        else if(add == 0)
        {
            //neutral answer
        }
        else
        {
            tempAffection--;
            if (tempAffection < 0) tempAffection = 0;
        }

        affectionsChange.Raise();
    }

    public void SetAffection(int i)
    {
        i = Mathf.Clamp(i, 0, 3);
        affectionLevel = i;
    }
}
