using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{
    public Button abilityOneButton;
    public Button abilityTwoButton;
    private Text abilityOneText;
    private Text abilityTwoText;

    public Image jobIcon;

    private Unit playerUnit;

    private void Start()
    {
        abilityOneText = abilityOneButton.gameObject.GetComponentInChildren<Text>();
        abilityTwoText = abilityTwoButton.gameObject.GetComponentInChildren<Text>();
    }

    public void ButtonCheck()
    {
        playerUnit = BattleSystem.instance.playerUnit;

        if (playerUnit.jobs[playerUnit.currentJobIdx].cooldownOne == 0)
            abilityOneButton.interactable = true;
        else
            abilityOneButton.interactable = false;

        if (playerUnit.jobs[playerUnit.currentJobIdx].cooldownTwo == 0)
            abilityTwoButton.interactable = true;
        else
            abilityTwoButton.interactable = false;
    }

    public void UpdateButtons()
    {
        ButtonCheck();
        playerUnit = BattleSystem.instance.playerUnit;

        abilityOneText.text = playerUnit.job.abilityOne.name;
        abilityTwoText.text = playerUnit.job.abilityTwo.name;
    }

    public void UpdateJobIcon()
    {
        jobIcon.sprite = BattleSystem.instance.playerUnit.job.icon;
    }
}
