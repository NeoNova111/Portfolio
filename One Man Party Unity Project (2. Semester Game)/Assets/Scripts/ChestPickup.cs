using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestPickup : Interactable
{
    public List<Job> jobs;
    public Sprite openedSprite;

    public override void Interact()
    {
        base.Interact();
        if (interactable)
        {
            interactable = false;

            foreach (Job job in jobs)
            {
                if (job != null)
                {
                    Inventory.instance.AddJob(job);
                    if(soundSource != null)
                        soundSource.Play();
                }
            }

            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
        interacting = false;
    }
}
