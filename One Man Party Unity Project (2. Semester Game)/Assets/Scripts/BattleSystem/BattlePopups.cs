using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePopups : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayerTurnTrigger()
    {
        animator.SetTrigger("Player Turn");
    }

    public void DeafeatTrigger()
    {
        animator.SetTrigger("Defeat");
    }

    public void VictoryTrigger()
    {
        animator.SetTrigger("Victory");
    }
}
