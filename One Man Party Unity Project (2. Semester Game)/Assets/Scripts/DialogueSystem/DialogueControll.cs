using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueControll : MonoBehaviour
{
    #region Singleton
    public static DialogueControll instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    public Dialogue dialogue;

    public GameObject speakerLeft;
    public GameObject speakerRight;
    public GameObject continueDialogueButon;

    private SpeakerUI speakerUILeft;
    private SpeakerUI speakerUIRight;

    private SpeakerUI activeSpeakerUI;

    private InteractibleNPC interactingNPC;

    int activeLineIdx = 0;
    public bool activeDialogue = false;

    void Start()
    {
        continueDialogueButon.SetActive(false);
        speakerUILeft = speakerLeft.GetComponent<SpeakerUI>();
        speakerUIRight = speakerRight.GetComponent<SpeakerUI>();

        speakerUILeft.Speaker = dialogue.leftSpeaker;
        speakerUIRight.Speaker = dialogue.rightSpeaker;

        activeSpeakerUI = speakerUILeft;
    }

    public void SetDialogue(Dialogue dialogue, InteractibleNPC interacting)
    {
        this.dialogue = dialogue;
        speakerUILeft.Speaker = this.dialogue.leftSpeaker;
        speakerUIRight.Speaker = this.dialogue.rightSpeaker;
        interactingNPC = interacting;

        activeSpeakerUI = speakerUILeft;
    }

    void Update()
    {
        if(activeDialogue && !continueDialogueButon.activeInHierarchy)
        {
            continueDialogueButon.SetActive(true);
        }
        else if (!activeDialogue && continueDialogueButon.activeInHierarchy)
        {
            continueDialogueButon.SetActive(false);
        }
    }

    public void AdvanceConversation()
    {
        if(activeLineIdx < dialogue.lines.Length && !activeSpeakerUI.isTyping)
        {
            DisplayLine();
            activeLineIdx++;
        }
        else if (activeSpeakerUI.isTyping)
        {
            activeSpeakerUI.SkipDialogueTyping();
        }
        else
        {
            EndDialogue();
            interactingNPC.interacting = false;
            if (interactingNPC.conversations[interactingNPC.GetDialogueIdx()].triggersNextDialogue)
                interactingNPC.NextDialogue();
        }
    }

    public void DisplayLine()
    {
        Line line = dialogue.lines[activeLineIdx];
        DialogueCharacter character = line.character;

        if (speakerUILeft.CompareSpeaker(character))
        {
            activeSpeakerUI = speakerUILeft;
            SetDialogue(speakerUILeft, speakerUIRight, line.text);
        }
        else
        {
            activeSpeakerUI = speakerUIRight;
            SetDialogue(speakerUIRight, speakerUILeft, line.text);
        }
    }

    public void EndDialogue()
    {
        speakerUILeft.gameObject.SetActive(false);
        speakerUIRight.gameObject.SetActive(false);
        activeDialogue = false;
        activeLineIdx = 0;

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().UnfreezeMovement();
    }

    void SetDialogue(SpeakerUI activeSpeaker, SpeakerUI inactiveSpeaker, string text)
    {
        activeSpeaker.gameObject.SetActive(true);
        inactiveSpeaker.gameObject.SetActive(false);
        activeSpeaker.SetDialogue(text);
    }
}
