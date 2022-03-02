using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterQueueManager : MonoBehaviour
{
    //public static CharacterQueueManager instance;

    //private void Awake()
    //{
    //    instance = this;
    //}

    //public Transform rightEntry;
    //public Transform leftEntry;
    //public Transform restTransform;

    //public DialogueCharacter[] charactersInQueue;
    ////QueueCharacter[] queueCharacters;
    //public QueueCharacter currentQueueCharacter;
    //public int currentCharcterIndex = 0; //save 

    //public GameObject queueCharacterPrefab;
    //[HideInInspector] public bool cycling = false;
    //public float cycleSpeed = 30;

    //GameManager gameManagerInstance;

    //void Start()
    //{
    //    gameManagerInstance = GameManager.instance;

    //    currentCharcterIndex = gameManagerInstance.queueIndex;
    //    EnterScreen(0, 1000);
    //    cycling = true;

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if(!currentQueueCharacter.Moving() && !DialogueSystem.instance.speechPanel.activeSelf)
    //    {
    //        if (Input.GetKey(KeyCode.F))
    //            NextCharacter();

    //        if (Input.GetKey(KeyCode.D))
    //            PreviousCharacter();
    //    }
    //}

    //void NextCharacter()
    //{
    //    cycling = true;
    //    LeaveScreen(0, cycleSpeed);
    //    currentCharcterIndex++;
    //    if(currentCharcterIndex >= charactersInQueue.Length) currentCharcterIndex = 0;
    //    EnterScreen(0, cycleSpeed);
    //}

    //void PreviousCharacter()
    //{
    //    cycling = true;
    //    LeaveScreen(1, cycleSpeed);
    //    currentCharcterIndex--;
    //    if (currentCharcterIndex < 0) currentCharcterIndex = charactersInQueue.Length - 1;
    //    EnterScreen(1, cycleSpeed);
    //}

    //void LeaveScreen(int direction, float speed)
    //{
    //    if (!currentQueueCharacter)
    //        return;

    //    if (direction == 0)
    //        currentQueueCharacter.MoveToTarget(leftEntry, speed);
    //    else
    //        currentQueueCharacter.MoveToTarget(rightEntry, speed);
    //}

    //void EnterScreen(int direction, float speed)
    //{
    //    GameObject obj;

    //    if (direction == 0)
    //        obj = Instantiate(queueCharacterPrefab, rightEntry.position, Quaternion.identity);
    //    else
    //        obj = Instantiate(queueCharacterPrefab, leftEntry.position, Quaternion.identity);

    //    currentQueueCharacter = obj.GetComponent<QueueCharacter>();
    //    currentQueueCharacter.entry = leftEntry;
    //    currentQueueCharacter.exit = rightEntry;
    //    currentQueueCharacter.destroy = true;
    //    currentQueueCharacter.SetCharacter(charactersInQueue[currentCharcterIndex]);
    //    currentQueueCharacter.MoveToTarget(restTransform, speed);
    //    DialogueSystem.instance.SetConvo(currentQueueCharacter.character.smallTalk);
    //    AffectionUI.instance.SetHearts();
    //    UpdateGamemanagerVariables();
    //}

    //public void UpdateGamemanagerVariables()
    //{
    //    gameManagerInstance.queueIndex = currentCharcterIndex;
    //    gameManagerInstance.currentSong = currentQueueCharacter.character.rhythmGameSong; //todo: combine song and character?
    //    gameManagerInstance.currentCharacter = currentQueueCharacter.character;
    //}
}
