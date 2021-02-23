using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueControll : MonoBehaviour
{
    #region Singleton
    public static DialogueControll instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null)
        {
            Debug.LogWarning("More than one dialogueControl instance");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion

    public Dialogue dialogue;

    public GameObject speakerLeft;
    public GameObject speakerRight;

    public GameEvent transmissionStart;
    public GameEvent transmissionEnd;

    public float delayAfterFinishedLine = 1;
    float totalDelay = 9999;

    private SpeakerUI speakerUILeft;
    private SpeakerUI speakerUIRight;

    private SpeakerUI activeSpeakerUI;

    int activeLineIdx = 0;
    bool activeDialogue = false;

    SoundManager voiceLineManager;
    DialogueTrigger trigger;

    void Start()
    {
        speakerUILeft = speakerLeft.GetComponent<SpeakerUI>();
        speakerUIRight = speakerRight.GetComponent<SpeakerUI>();

        speakerUILeft.Speaker = dialogue.leftSpeaker;
        speakerUIRight.Speaker = dialogue.rightSpeaker;

        activeSpeakerUI = speakerUILeft;

        voiceLineManager = AudioManager.instance.soundManager_voiceLines;
    }

    //poorly named: this one is for initially setting up speaker UI
    public void SetDialogue(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        speakerUILeft.Speaker = this.dialogue.leftSpeaker;
        speakerUIRight.Speaker = this.dialogue.rightSpeaker;

        activeSpeakerUI = speakerUILeft;
        transmissionStart.Raise();
    }

    void Update()
    {
        Debug.Log(activeSpeakerUI.isTyping);

        //redunsant second if, but "eh"
        if (activeSpeakerUI != null)
        {
            if (Input.GetKeyDown("space") && activeDialogue)
            {
                AdvanceConversation();
            }
        }

        if (activeDialogue)
        {
            totalDelay -= Time.deltaTime;
            if (!activeSpeakerUI.CurrentlyTyping() && totalDelay <= 0)
            {
                AdvanceConversation();
            }
        }
    }

    public void TriggerDialogue(Dialogue dialogue)
    {
        SetDialogue(dialogue);
        AdvanceConversation();
    }

    public bool AdvanceConversation() //returns false at end of conversation
    {

        activeDialogue = true;
        if(activeLineIdx < dialogue.lines.Length && !activeSpeakerUI.isTyping && !voiceLineManager.IsAnyPlaying())
        {
            DisplayLine();

            activeLineIdx++;
            return true;
        }
        else if (activeSpeakerUI.isTyping || voiceLineManager.IsAnyPlaying())
        {
            activeSpeakerUI.SkipDialogueTyping();
            voiceLineManager.StopAllClipsPlaying();
            return true;
        }
        else
        {
            EndDialogue();
            return false;
        }
    }

    void DisplayLine()
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

        if (dialogue.lines[activeLineIdx].hasVoceOver)
        {
            voiceLineManager.PlayClip(dialogue.lines[activeLineIdx].voiceLine);
            totalDelay = voiceLineManager.LengthOfClip(0) + delayAfterFinishedLine;
        }
        else
        {
            totalDelay = activeSpeakerUI.AnticipatedTypingTime() + delayAfterFinishedLine;
        }
    }

    public void EndDialogue()
    {
        speakerUILeft.gameObject.SetActive(false);
        speakerUIRight.gameObject.SetActive(false);
        activeDialogue = false;
        activeLineIdx = 0;

        if (dialogue.triggersNextDialogue && trigger)
            trigger.NextDialogue();

        transmissionEnd.Raise();
    }

    //poorly named: this one is for setting up speaker UI
    void SetDialogue(SpeakerUI activeSpeaker, SpeakerUI inactiveSpeaker, string text)
    {
        activeSpeaker.gameObject.SetActive(true);
        inactiveSpeaker.gameObject.SetActive(false);
        activeSpeaker.SetDialogue(text);
    }

    public void Pause()
    {
        //if (!voiceLineManager)
        //    return;

        //voiceLineManager.StopAllClipsPlaying();
    }

    public void Unpause()
    {
        //if (!voiceLineManager)
        //    return;

        //activeLineIdx--;
        //AdvanceConversation();
    }

    public void SetTrigger(DialogueTrigger trigger)
    {
        this.trigger = trigger;
    }

    public bool HasActiveDialogue()
    {
        return activeDialogue;
    }

    public void SetActiveDialogue(bool b)
    {
        activeDialogue = b;
    }
}
