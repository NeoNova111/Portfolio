using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleNPC : Interactable
{
    public Dialogue[] conversations;
    [SerializeField]
    private int dialogueIdx = 0;
    private DialogueControll dialogueControl;

    private void Start()
    {
        dialogueControl = DialogueControll.instance;
    }

    public override void Interact()
    {
        base.Interact();
        EnterDialogue();
        interacting = true;
    }

    void EnterDialogue()
    {
        if (!DialogueControll.instance.activeDialogue)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().FreezeMovement();
            dialogueControl.SetDialogue(conversations[dialogueIdx], this);
            dialogueControl.activeDialogue = true;
        }

        dialogueControl.AdvanceConversation();
    }

    public void NextDialogue()
    {
        if (dialogueIdx + 1 < conversations.Length)
            dialogueIdx++;
        else
            Debug.Log("Reached last dialogue for " + gameObject.name);
    }

    public int GetDialogueIdx()
    {
        return dialogueIdx;
    }
}
