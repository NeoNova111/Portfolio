using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float interactRange;
    public bool inRange = false;
    public bool interactable = true;
    public bool interacting;
    public GameObject interactCanvas;
    public AudioSource soundSource;

    private void Start()
    {
        interacting = false;
        interactCanvas.SetActive(false);
    }

    public void Update()
    {
        if(soundSource != null)
            soundSource.volume = AudioManager.instance.masterVolume;

        if (Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) <= interactRange && interactable)
        {
            ShowPromt();
            //UI.instance.interactText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown("space"))
            {
                Interact();
            }
        }
        else
        {
            HidePrompt();
        }
    }

    public virtual void Interact() { }

    public void ShowPromt()
    {
        if (!interacting)
            interactCanvas.SetActive(true);
        else
            HidePrompt();
    }

    public void HidePrompt()
    {
        interactCanvas.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
