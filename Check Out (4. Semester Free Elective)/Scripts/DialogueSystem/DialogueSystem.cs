using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;
    private void Awake()
    {
        instance = this;
    }

    [System.Serializable] public struct ELEMENTS
    {
        public GameObject speechPanel;
        public Text nameText;
        public Text speechText;

        public GameObject choicePanel;
        public Text choiceOne;
        public Text choiceTwo;

        public Image characterImage;
        public GameObject tempAffectionUI;
        //public GameObject characterInfo;
    }
    public GameObject speechPanel { get { return elements.speechPanel; } }
    public Text nameText { get { return elements.nameText; } }
    public Text speechText { get { return elements.speechText; } }
    public GameObject choicePanel { get { return elements.choicePanel; } }
    public Text choiceOne { get { return elements.choiceOne; } }
    public Text choiceTwo { get { return elements.choiceTwo; } }
    public Image characterImage { get { return elements.characterImage; } }
    public GameObject tempAffectionUI { get { return elements.tempAffectionUI;  } }
    //public GameObject characterInfo { get { return elements.characterInfo;  } }
    //public Sprite characterSprite { get { return elements.characterSprite; } }
    public ELEMENTS elements;

    public DialogueConversation currentConverstaion;

    public KeyCode advanceDialogueKey;
    [HideInInspector] public bool isWaitingForChoice = false; //not used rn
    public float charactersPerSecond;

    string targetSpeech = "";
    Coroutine speaking = null;
    bool reachedEnd = false;
    public bool isSpeaking { get { return speaking != null; } }
    GameManager gameManager;

    public GameEvent levelStart;
    public GameEvent dialogueEnded;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;

        currentConverstaion.lineIndex = 0;
        DeactivateUI();

        if (gameManager.cameFromGame)
        {
            if (gameManager.lastGameSuccess)
                SetConvo(gameManager.currentCharacter.gameSuccess);
            else
                SetConvo(gameManager.currentCharacter.gameFail);

            SayCurentLine();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(advanceDialogueKey) && !ChibiQueue.instance.moving && !gameManager.menu)
        {
            if(!speechPanel.activeSelf)
            {
                levelStart.Raise();
            }

            SayCurentLine();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(isWaitingForChoice);
        }
    }

    public void SayCurentLine()
    {
        if(ChibiQueue.instance.moving)
        {
            return;
        }

        //restets index when startin new dialogue
        if (!speechPanel.activeSelf)
        {
            currentConverstaion.lineIndex = 0;
        }

        Say(currentConverstaion.currentLine().text, currentConverstaion.currentLine().dialogueType, currentConverstaion.dialogueCharacter.characterName);
    }

    public void Say(string dialogue, DialogueType type, string speakerName) 
    {
        if(isSpeaking)
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


            speaking = StartCoroutine(Speaking(dialogue, type, speakerName));
            //set sprite here
            SetCharacterSprite(currentConverstaion.conversationLines[currentConverstaion.lineIndex].spriteType);
            reachedEnd = !currentConverstaion.IterateLineIndex();
        }
    }

    public void SetCharacterSprite(CharacterSpriteType type)
    {
        //don't know how good of an idea it is to cast enum to int, might be confusing
        if ((int)type >= currentConverstaion.dialogueCharacter.characterSprites.Length)
        {
            characterImage.sprite = currentConverstaion.dialogueCharacter.characterSprites[0];
            Debug.LogWarning("falling back onto default sprite");
        }
        else
        {
            characterImage.sprite = currentConverstaion.dialogueCharacter.characterSprites[(int)type];
        }
    }

    public bool StopSpeaking()
    {
        bool stopped = false;
        speechText.text = targetSpeech;

        if(isWaitingForChoice)
        {
            choicePanel.SetActive(true);
        }

        if (isSpeaking)
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

        StartCoroutine(DialogueTrulyEnded());
    }

    IEnumerator DialogueTrulyEnded()
    {
        yield return new WaitForEndOfFrame();
        if (!speechPanel.activeSelf)
        {
            dialogueEnded.Raise();
        }
    }

    IEnumerator Speaking(string speech, DialogueType dialogueType, string speakerName)
    {
        //just to be sure everything is set up right
        ActivateUI();
        targetSpeech = speech;
        //isWaitingForChoice = false;
        //isSpeaking = true;

        switch (dialogueType)
        {
            case DialogueType.ADDITIVE:
                targetSpeech = speechText.text + targetSpeech;
                break;
            case DialogueType.OVERRIDE:
                speechText.text = "";
                break;
            case DialogueType.CHOICE:
                speechText.text = "";
                isWaitingForChoice = true;
                choiceOne.text = currentConverstaion.currentLine().choices[0].choiceText;
                choiceTwo.text = currentConverstaion.currentLine().choices[1].choiceText;
                break;
        }
        nameText.text = speakerName; //temp

        for(int i = speechText.text.Length; i < targetSpeech.Length; i++)
        {
            speechText.text += targetSpeech[i];
            yield return new WaitForSecondsRealtime(1/charactersPerSecond);
        }

        //finished Text
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
        choicePanel.SetActive(false);

        currentConverstaion.dialogueCharacter.AddAffection(currentConverstaion.currentLine().choices[i].affectionConsequence); //make this cleaner maybe
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
        characterImage.gameObject.SetActive(true);
        choicePanel.SetActive(false);
        tempAffectionUI.SetActive(true);
    }

    void DeactivateUI()
    {
        speechPanel.SetActive(false);
        characterImage.gameObject.SetActive(false);
        choicePanel.gameObject.SetActive(false);
        tempAffectionUI.SetActive(false);
    }
}