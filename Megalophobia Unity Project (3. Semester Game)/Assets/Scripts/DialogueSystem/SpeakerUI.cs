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
    public float typingDelay = 0.02f;

    public GameEvent typingEvent;

    private string dialogueText = "";
    private int typingPosition;

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

    private void OnEnable()
    {
        if (isTyping)
        {
            UnpauseTyping();
        }
    }

    public void SetDialogue(string dialogue)
    {
        typingPosition = 0;
        dialogueTextField.text = "";

        dialogueText = dialogue;
        coroutineInst = TypeTextCoroutine(dialogueTextField, dialogueText);
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
        float soundDelay = 0;

        isTyping = true;
        for(int i = typingPosition; i < dialogueText.Length; i++)
        {
            textfieldToTypeIn.text += dialogueText[i];
            typingPosition = i;

            if (soundDelay == 0)
            {
                typingEvent.Raise();
                soundDelay = 3;
            }
            else
            {
                soundDelay--;
            }

            yield return new WaitForSeconds(typingDelay);
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

    public bool CurrentlyTyping()
    {
        return isTyping;
    }

    public float AnticipatedTypingTime()
    {
        return typingDelay * dialogueText.Length;
    }

    public void PauseTyping()
    {
        StopCoroutine(coroutineInst);
    }

    public void UnpauseTyping()
    {
        coroutineInst = TypeTextCoroutine(dialogueTextField, dialogueText);
        StartCoroutine(coroutineInst);
    }

    private void OnDisable()
    {
        StopCoroutine(coroutineInst);
    }
}
