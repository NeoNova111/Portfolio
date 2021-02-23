using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singelton
    public static Inventory instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null)
        {
            Debug.LogWarning("More than one inventory instance");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    #endregion

    public List<Job> jobsInInventory = new List<Job>();
    public List<Job> equipedJobs = new List<Job>();

    public int totalSlots = 16;
    public int equipSlots = 3;

    public GameEvent InventoryChanged;
    public GameEvent JobEquip;
    public GameEvent JobUnequip;

    public bool AddJob(Job job)
    {
        if (jobsInInventory.Contains(job) || equipedJobs.Contains(job))
        {
            Debug.Log("you already aquired that job");
            return false;
        }

        if (equipedJobs.Count < 3)
        {
            equipedJobs.Add(job);
            InventoryChanged.Raise();
            return true;
        }

        if (equipedJobs.Count + jobsInInventory.Count < totalSlots)
        {
            jobsInInventory.Add(job);
            InventoryChanged.Raise();
            return true;
        }

        Debug.Log("Inventory is full");
        return false;
    }

    public void ChangeEquipedState(Job job)
    {
        if (jobsInInventory.Contains(job))
        {
            EquipJob(job);
            return;
        }

        if (equipedJobs.Contains(job))
        {
            UneqipJob(job);
            return;
        }

        Debug.LogWarning("Something went wrong while equipinmg/unequiping");
    }

    public void EquipJob(Job job)
    {
        if (equipedJobs.Count == 3)
        {
            Debug.Log("You can't equip any more");
            return;
        }

        jobsInInventory.Remove(job);
        equipedJobs.Add(job);
        InventoryChanged.Raise();
        JobEquip.Raise();
    }

    public void UneqipJob(Job job)
    {
        if (!equipedJobs.Contains(job) || equipedJobs.Count == 1)
        {
            Debug.Log("Job has to be equiped to unequip or only one job left");
            return;
        }

        equipedJobs.Remove(job);
        jobsInInventory.Add(job);
        InventoryChanged.Raise();
        JobUnequip.Raise();
    }
}
