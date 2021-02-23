using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveTrigger : MonoBehaviour
{
    public bool forcedDive = false;
    public bool moveWhileDiving = true;

    bool diving;
    BoxCollider coll;

    submarineMovement subInstance;
    AbilityUI abilityUI;

    private void Start()
    {
        diving = false;
        coll = GetComponent<BoxCollider>();
        subInstance = submarineMovement.instance;
        abilityUI = UIManager.instance.abilityUI;
    }

    private void Update()
    {
        if (diving && subInstance.transform.position.y <= transform.position.y - (coll.size.y / 2) * transform.lossyScale.y)
        {
            EndDive();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !diving)
        {
            if(!forcedDive)
                ShowUI(true);

            if (forcedDive || Input.GetKeyDown(KeyCode.Y))
                StartDive();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            ShowUI(false);
        }
    }

    void StartDive()
    {
        diving = true;
        subInstance.SetDiving(true);
        ShowUI(false);
    }

    void EndDive()
    {
        subInstance.SetDiving(false);
    }

    void ShowUI(bool b)
    {
        abilityUI.ShowDivingPrompt(b);
    }
}
