using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform Transform { get => overrideTransform ? overrideTransform : transform; }
    public bool IsEntrance { get => isEntrance; set => isEntrance = value; }

    [SerializeField] private bool open = false;
    [SerializeField] private GameObject blockage;
    [SerializeField] private Transform overrideTransform = null;

    private bool isEntrance = false;

    void Start()
    {
        if (open) OpenDoor();
        else CloseDoor();

        if(blockage == null)
        {
            //getting the first child is usually fine
            blockage = transform.GetChild(0).gameObject;
        }
    }

    public void OpenDoor()
    {
        if (!blockage) return;

        blockage.SetActive(false);
        open = true;
    }

    public void CloseDoor()
    {
        if (!blockage) return;

        blockage.SetActive(true);
        open = false;
    }
}
