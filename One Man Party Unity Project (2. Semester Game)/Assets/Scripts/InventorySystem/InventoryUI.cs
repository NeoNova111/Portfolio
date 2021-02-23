using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform uneqipedParent;
    public Transform equipedParent;
    public GameObject openButton;
    public GameObject closeButton;

    Inventory inventory;

    InventorySlot[] slots;
    InventorySlot[] equipedSlots;

    private bool open;


    // Start is called before the first frame update
    void Start()
    {
        open = false;
        CloseInventory();
        inventory = Inventory.instance;

        slots = uneqipedParent.GetComponentsInChildren<InventorySlot>();
        equipedSlots= equipedParent.GetComponentsInChildren<InventorySlot>();

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (open)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.jobsInInventory.Count)
            {
                slots[i].AddJob(inventory.jobsInInventory[i]);
            }else
            {
                slots[i].ClearSlot();
            }
        }

        for (int i = 0; i < equipedSlots.Length; i++)
        {
            if (i < inventory.equipedJobs.Count)
            {
                equipedSlots[i].AddJob(inventory.equipedJobs[i]);
            }
            else
            {
                equipedSlots[i].ClearSlot();
            }
        }
    }

    public void OpenInventory()
    {
        uneqipedParent.gameObject.SetActive(true);
        openButton.SetActive(false);
        closeButton.SetActive(true);
        open = true;
    }

    public void CloseInventory()
    {
        uneqipedParent.gameObject.SetActive(false);
        openButton.SetActive(true);
        closeButton.SetActive(false);
        open = false;
    }
}
