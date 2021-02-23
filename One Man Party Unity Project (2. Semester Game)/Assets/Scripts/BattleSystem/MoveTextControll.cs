using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveTextControll : MonoBehaviour
{
    private Unit player;
    public Text dialogueText;
    private JobHolder playerEquipedJob;

    public BattleSystem battleSystem;

    private string previousText;

    private void Start()
    {
        player = BattleSystem.instance.playerUnit;
    }

    public void Update()
    {
        playerEquipedJob = player.jobs[player.currentJobIdx];
    }

    public void DisplayAbilityOneText()
    {
        if (battleSystem.battleState == BattleState.PLAYERACTIONCHOICE)
        {
            previousText = dialogueText.text;
            if(playerEquipedJob.cooldownOne == 0)
                dialogueText.text = player.job.abilityOne.description;
            else
                dialogueText.text = "On cooldown for "+ playerEquipedJob.cooldownOne + " more turns";
        }
    }

    public void DisplayAbilityTwoText()
    {
        if (battleSystem.battleState == BattleState.PLAYERACTIONCHOICE)
        {
            previousText = dialogueText.text;
            if (playerEquipedJob.cooldownTwo == 0)
                dialogueText.text = player.job.abilityTwo.description;
            else
                dialogueText.text = "On cooldown for " + playerEquipedJob.cooldownTwo + " more turns";
        }
    }

    public void RevertText()
    {
        if (battleSystem.battleState == BattleState.PLAYERACTIONCHOICE)
        {
            dialogueText.text = previousText;
        }
    }
}
