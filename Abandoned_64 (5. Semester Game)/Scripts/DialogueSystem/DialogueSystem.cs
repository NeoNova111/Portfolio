using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using System.Text.RegularExpressions;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;
    private void Awake()
    {
        Instance = this;
    }

    [System.Serializable]
    public struct ELEMENTS
    {
        public GameObject speechPanel;
        public TextMeshProUGUI speechText;
        public Animator speechPanelAnimator;

        public GameObject choicePanel;
        public TextMeshProUGUI choiceOne;
        public TextMeshProUGUI choiceTwo;
    }
    public GameObject speechPanel { get { return elements.speechPanel; } }
    public TextMeshProUGUI speechText { get { return elements.speechText; } }
    public Animator speechPanelAnimator { get { return elements.speechPanelAnimator; } }
    public GameObject choicePanel { get { return elements.choicePanel; } }
    public TextMeshProUGUI choiceOne { get { return elements.choiceOne; } }
    public TextMeshProUGUI choiceTwo { get { return elements.choiceTwo; } }

    public ELEMENTS elements;

    public DialogueConversation currentConverstaion;
    public bool ActiveDialogue { get => speechPanel.activeSelf; }

    public KeyCode advanceDialogueKey;
    private bool isWaitingForChoice = false;
    public bool IsWaitingForChoice { get => isWaitingForChoice; }
    public float charactersPerSecond;
    public int everyNthCharacterSound = 3;
    private int nthCharcter;

    string targetSpeech = "";
    Coroutine speaking = null;
    bool reachedEnd = false;
    public bool ReachedEnd { get => reachedEnd; }
    public bool IsSpeaking { get { return speaking != null; } }

    public GameEvent dialogueEnded;
    public GameEvent dialogueStarted;
    public GameEvent NewCharacter;

    private CinemachineVirtualCamera activeVirtualCamera;

    // Start is called before the first frame update
    void Start()
    {
        if(currentConverstaion)
            currentConverstaion.lineIndex = 0;

        speechPanel.SetActive(false);
        choicePanel.gameObject.SetActive(false);
    }

    public void SayCurentLine()
    {

        //restets index when startin new dialogue
        if (!speechPanel.activeSelf)
        {
            currentConverstaion.lineIndex = 0;
            nthCharcter = everyNthCharacterSound;
            dialogueStarted.Raise();
        }

        Say(currentConverstaion.currentLine().text, currentConverstaion.currentLine().dialogueType);
    }

    public void SayCurentLine(SignNPC npcSayingThis)
    {
        //restets index when startin new dialogue
        if (!speechPanel.activeSelf)
        {
            currentConverstaion.lineIndex = 0;
            nthCharcter = everyNthCharacterSound;
            dialogueStarted.Raise();
        }

        Say(currentConverstaion.currentLine().text, currentConverstaion.currentLine().dialogueType, npcSayingThis);
    }

    public void Say(string dialogue, DialogueType type)
    {
       
        if (IsSpeaking)
        {
            StopSpeaking();
        }
        else
        {
            if (isWaitingForChoice) //can't advance dialogue if waiting for choice
            {
                return;
            }

            if (reachedEnd) //ends dialogue if at the end and no one is typing/ speaking
            {
                EndDialogue();
                return;
            }


            speaking = StartCoroutine(Speaking(dialogue, type));
            reachedEnd = !currentConverstaion.IterateLineIndex();
        }
    }

    public void Say(string dialogue, DialogueType type, SignNPC npcSayingThis)
    {
        
        if (IsSpeaking)
        {
            StopSpeaking();
        }
        else
        {
            
            if (isWaitingForChoice) //can't advance dialogue if waiting for choice
            {
                return;
            }

            if (reachedEnd) //ends dialogue if at the end and no one is typing/ speaking
            {
                EndDialogue();
                return;
            }

            if (npcSayingThis.DialogueCameras.Length != 0 && currentConverstaion.currentLine().specialCameraAngleIndex < npcSayingThis.DialogueCameras.Length)
            {
                int specialCamIdx = currentConverstaion.currentLine().specialCameraAngleIndex;
                if (specialCamIdx < 0 || npcSayingThis.DialogueCameras[specialCamIdx] == null) //last cond
                {
                    DeactivateCurrentActiveVC();
                }
                else if(activeVirtualCamera != npcSayingThis.DialogueCameras[specialCamIdx])
                {
                    DeactivateCurrentActiveVC();
                    activeVirtualCamera = npcSayingThis.DialogueCameras[specialCamIdx];
                    activeVirtualCamera.gameObject.SetActive(true);
                    activeVirtualCamera.Priority = 100;
                }
            }

            speaking = StartCoroutine(Speaking(dialogue, type));
            reachedEnd = !currentConverstaion.IterateLineIndex();
        }
    }

    public bool StopSpeaking()
    {
        bool stopped = false;
        speechText.maxVisibleCharacters = targetSpeech.Length;

        if (isWaitingForChoice)
        {
            choicePanel.SetActive(true);
        }

        if (IsSpeaking)
        {
            stopped = true;
            //isSpeaking = false;
            StopCoroutine(speaking);
        }

        speaking = null;

        return stopped;
    }

    public void EndDialogue()
    {
        reachedEnd = false;
        StopSpeaking();
        DeactivateUI();
        //SaveManager.instance.SaveGameData();

        foreach (GameEvent e in currentConverstaion.conversationLines[currentConverstaion.lineIndex].triggerOnComplete)
        {
            e.Raise();
        }

        DeactivateCurrentActiveVC();

        dialogueEnded.Raise();
        //StartCoroutine(DialogueTrulyEnded());
    }

    void DeactivateCurrentActiveVC()
    {
        if (activeVirtualCamera)
        {
            activeVirtualCamera.Priority = 0;
            activeVirtualCamera.gameObject.SetActive(false);
            activeVirtualCamera = null;
        }
    }

    IEnumerator Speaking(string speech, DialogueType dialogueType)
    {
        //just to be sure everything is set up right
        ActivateUI();
        int alreadyVisibleChars;

        switch (dialogueType)
        {
            case DialogueType.ADDITIVE:
                targetSpeech = speechText.text + speech;
                alreadyVisibleChars = speechText.text.Length;
                break;
            case DialogueType.OVERRIDE:
                //speechText.text = "";
                alreadyVisibleChars = 0;
                targetSpeech = speech;
                break;
            case DialogueType.CHOICE:
                //speechText.text = "";
                alreadyVisibleChars = 0;
                targetSpeech = speech;
                isWaitingForChoice = true;
                //Cursor.lockState = CursorLockMode.Confined;
                choiceOne.text = currentConverstaion.currentLine().choices[0].choiceText;
                choiceTwo.text = currentConverstaion.currentLine().choices[1].choiceText;
                break;
            default:
                targetSpeech = speech;
                alreadyVisibleChars = 0;
                break;
        }

        int targetLength = Regex.Replace(targetSpeech, @"\<[^<>]*\>", string.Empty).Length;
        speechText.text = targetSpeech;

        for (int currentVisibleChars = alreadyVisibleChars; currentVisibleChars <= targetLength; currentVisibleChars++)
        {
            if(everyNthCharacterSound == nthCharcter)
            {
                NewCharacter.Raise();
                nthCharcter = 1;
            }
            else
            {
                nthCharcter++;
            }

            speechText.maxVisibleCharacters = currentVisibleChars;
            yield return new WaitForSecondsRealtime(1 / charactersPerSecond);
        }

        if (isWaitingForChoice)
        {
            choicePanel.SetActive(true);
        }

        while (isWaitingForChoice)
        {
            yield return new WaitForEndOfFrame();
        }

        StopSpeaking();
    }

    public void MakeChoice(int i)
    {
        isWaitingForChoice = false;
        Cursor.lockState = CursorLockMode.Locked;
        choicePanel.SetActive(false);

        if (currentConverstaion.currentLine().choices[i].eventConsequence)
            currentConverstaion.currentLine().choices[i].eventConsequence.Raise();

        if (currentConverstaion.currentLine().choices[i].dialogueConsequence)
        {
            SetConvo(currentConverstaion.currentLine().choices[i].dialogueConsequence);
            StopSpeaking();
            SayCurentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    public void ChoiceOne()
    {
        MakeChoice(0);
    }

    public void ChoiceTwo()
    {
        MakeChoice(1);
    }

    public void SetConvo(DialogueConversation convo)
    {
        currentConverstaion = convo;
        convo.lineIndex = 0;
        reachedEnd = false;
    }

    void ActivateUI()
    {
        speechPanel.SetActive(true);
        choicePanel.SetActive(false);
    }

    void DeactivateUI()
    {
        //speechPanel.SetActive(false);
        speechPanelAnimator.SetTrigger("DialogueEnd");
        choicePanel.gameObject.SetActive(false);
    }
}