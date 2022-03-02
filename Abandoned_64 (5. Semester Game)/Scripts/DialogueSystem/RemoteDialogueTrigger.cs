using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RemoteDialogueTrigger : MonoBehaviour
{
    [SerializeField] private SignNPC npcToForceDialogueWith;
    [SerializeField][Tooltip("-1 if you want infinite triggers")] private int triggerAmount = 1;
    private PlayerStateMachine player;
    private Collider coll;

    private void Start()
    {
        coll = GetComponent<Collider>();
        player = PlayerStateMachine.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if( other.tag == "Player" && triggerAmount > 0 && npcToForceDialogueWith && npcToForceDialogueWith.Interactable || 
            other.tag == "Player" && triggerAmount < 0 && npcToForceDialogueWith && npcToForceDialogueWith.Interactable)
        {
            player.ForceInteract(npcToForceDialogueWith.GetComponent<IInteractable>());
            triggerAmount--;
        }
    }

    public void DeactivateTrigger()
    {
        coll.isTrigger = false;
    }
}
