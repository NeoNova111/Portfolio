using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChibiQueue : MonoBehaviour
{
    #region Singleton
    public static ChibiQueue instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one CQ instance: destroying new one");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion

    public DialogueCharacter[] characters;
    public Transform[] queuePositions;
    QueueCharacter[] queueChibis;

    public GameObject queueCharacterPrefab;
    public GameEvent moveQueue;
    public GameEvent queueStopped;
    public GameEvent varsSet;
    public bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        queueChibis = new QueueCharacter[characters.Length];
        PopulateChibis();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            MoveQueue();
        }

        if (moving && !queueChibis[CharacterIndex()].Moving())
        {
            moving = false;
            queueStopped.Raise();
        }
    }

    void MoveQueue()
    {
        if (!Moving() && !DialogueSystem.instance.speechPanel.activeSelf && !GameManager.instance.menu)
        {
            moveQueue.Raise();
            moving = true;
            foreach (QueueCharacter chichar in queueChibis)
            {
                chichar.queuePos++;

                if(chichar.queuePos == 5)
                {
                    UpdateGamemanagerVariables(chichar);
                    DialogueSystem.instance.SetConvo(chichar.character.smallTalk);
                }

                chichar.MoveToTarget(queuePositions[chichar.queuePos], 20);

                if (chichar.queuePos == queuePositions.Length - 1)
                    chichar.queuePos = 0;

            }
        }
    }

    void PopulateChibis()
    {
        int startCharaIndex = CharacterIndex();
        for (int i = queuePositions.Length - 2; i >= 0; i--)
        {
            GameObject obj = Instantiate(queueCharacterPrefab, queuePositions[i].position, Quaternion.identity);
            QueueCharacter qC = obj.GetComponent<QueueCharacter>();
            qC.SetCharacter(characters[startCharaIndex]);
            qC.SetSprite(characters[startCharaIndex].chibiSprite);
            qC.entry = queuePositions[0];
            qC.queuePos = i;
            qC.PlaceAtTarget(queuePositions[i]);
            qC.exit = queuePositions[queuePositions.Length - 1];

            queueChibis[(queuePositions.Length - 2) - i] = qC;

            if(i == 5 && !GameManager.instance.cameFromGame)
            {
                UpdateGamemanagerVariables(qC);
                queueStopped.Raise();
                if (!DialogueSystem.instance.speechPanel.activeSelf)
                {
                    DialogueSystem.instance.SetConvo(qC.character.smallTalk);
                }
            }

            startCharaIndex++;
            if(startCharaIndex >= queueChibis.Length)
            {
                startCharaIndex = 0;
            }
        }
    }

    public bool Moving()
    {
        foreach(QueueCharacter chichar in queueChibis)
        {
            if (chichar.Moving())
                return true;
        }
        return false;
    }

    public void UpdateGamemanagerVariables(QueueCharacter currentQueueCharacter)
    {
        GameManager gameManagerInstance = GameManager.instance;
        //gameManagerInstance.queueIndex = currentCharcterIndex;
        gameManagerInstance.currentSong = currentQueueCharacter.character.rhythmGameSong; //todo: combine song and character?
        gameManagerInstance.currentCharacter = currentQueueCharacter.character;
        varsSet.Raise();
    }

    int CharacterIndex()
    {
        for(int i = 0; i < characters.Length; i++)
        {
            if(characters[i] == GameManager.instance.currentCharacter)
            {
                Debug.Log(i);
                return i;
            }
        }
        return 0;
    }
}
