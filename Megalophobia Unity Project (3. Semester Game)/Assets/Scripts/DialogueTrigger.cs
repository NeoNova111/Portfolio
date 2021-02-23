using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue[] conversations;
    public bool endsCurrentDialogue;
    public float triggerDelayTime = 10f;
    //maybe add triggeramount to each dialogue of conversation
    public int triggerAmount = 1;
    float delay;

    [SerializeField]
    private int dialogueIdx = 0; //serialize that for save files
    bool ableToTrigger = true;

    DialogueControll dialogueControl;

    private void Start()
    {
        dialogueControl = DialogueControll.instance;
        delay = 0;
    }

    private void Update()
    {
        delay -= Time.deltaTime;
    }

    void EnterDialogue()
    {
        triggerAmount--;
        if (dialogueIdx < conversations.Length)
        {
            if (!dialogueControl.HasActiveDialogue() || endsCurrentDialogue)
            {
                if(dialogueControl.HasActiveDialogue())
                    dialogueControl.EndDialogue();

                dialogueControl.SetDialogue(conversations[dialogueIdx]);
                dialogueControl.SetTrigger(this);
                //dialogueControl.SetActiveDialogue(true);
                dialogueControl.AdvanceConversation();
            }
        }
    }

    public void NextDialogue()
    {
        dialogueIdx++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && delay <= 0 && triggerAmount > 0)
            EnterDialogue();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && delay <= 0)
        {
            delay = triggerDelayTime;
        }
    }
}