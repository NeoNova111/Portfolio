using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackInventory : MonoBehaviour
{
    public int currentClassIdx;
    public Job[] classInventory;
    private BattleSystem system;

    public GameEvent NewJob;

    void Awake()
    {
        if (Inventory.instance != null)
        {
            classInventory = new Job[Inventory.instance.equipedJobs.Count];
            for(int i = 0; i < classInventory.Length; i++)
            {
                classInventory[i] = Inventory.instance.equipedJobs[i];
            }
        }
    }

    private void Start()
    {
        system = BattleSystem.instance;
        currentClassIdx = 0;
        EquipNewClass();
    }

    public void NextClass()
    {
        if (system.battleState == BattleState.PLAYERACTIONCHOICE)
        {
            if (currentClassIdx == classInventory.Length - 1)
            {
                currentClassIdx = 0;
            }
            else
            {
                currentClassIdx += 1;
            }
            EquipNewClass();
        }
    }

    public void PreviousClass()
    {
        if (system.battleState == BattleState.PLAYERACTIONCHOICE)
        {
            if (currentClassIdx == 0)
            {
                currentClassIdx = classInventory.Length - 1;
            }
            else
            {
                currentClassIdx -= 1;
            }
            EquipNewClass();
        }
    }

    void EquipNewClass()
    {
        Unit player = system.playerUnit;
        player.currentJobIdx = currentClassIdx;
        player.job = classInventory[currentClassIdx];
        system.SetPlayerUnit(player);
        NewJob.Raise();
        system.UpdateHud();
    }
}
