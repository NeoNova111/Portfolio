using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerUI : MonoBehaviour
{
    public Image portrait;
    public Text characterName;
    public Text dialogueTextField;
    public bool isTyping;

    private string dialogueText = "";

    IEnumerator coroutineInst = null;

    //might be bad practice
    private DialogueCharacter speaker;
    public DialogueCharacter Speaker
    {
        get { return speaker; }
        set
        {
            speaker = value;
            portrait.sprite = speaker.portrait;
            characterName.text = speaker.characterName;
        }
    }

    public void SetDialogue(string dialogue)
    {
        dialogueText = dialogue;
        coroutineInst = TypeTextCoroutine(dialogueTextField, dialogue);
        StartCoroutine(coroutineInst);
    }

    public void SkipDialogueTyping()
    {
        StopCoroutine(coroutineInst);
        isTyping = false;
        dialogueTextField.text = dialogueText;
    }

    IEnumerator TypeTextCoroutine(Text textfieldToTypeIn, string textToType)
    {
        isTyping = true;
        textfieldToTypeIn.text = "";
        foreach (char character in textToType)
        {
            textfieldToTypeIn.text += character;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;
    }

    public bool HasSpeaker()
    {
        return speaker != null;
    }

    public bool CompareSpeaker(DialogueCharacter character)
    {
        return speaker == character;
    }
}
