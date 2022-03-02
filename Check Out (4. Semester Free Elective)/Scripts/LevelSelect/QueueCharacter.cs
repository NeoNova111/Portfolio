using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueCharacter : MonoBehaviour
{
    public DialogueCharacter character;
    SpriteRenderer characterSprite;
    public bool destroy = false;
    float moveSpeed = 10;

    public int queuePos = 0;
    public Transform MoveTarget { get { return moveTarget; } }
    Transform moveTarget;

    //public bc I'm lazy for this project
    public Transform exit;
    public Transform entry;

    //must have awake instead of start for some reason otherwise set manually in inspector
    private void Awake()
    {
        characterSprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //moveTarget = transform;
    }

    void Update()
    {
        if (DialogueSystem.instance.speechPanel.activeSelf)
            characterSprite.enabled = false;
        else
            characterSprite.enabled = true;

        if (transform.position != moveTarget.position)
            transform.position = Vector3.MoveTowards(transform.position, moveTarget.position, moveSpeed * Time.deltaTime);
        else if (transform.position == exit.position || transform.position == entry.position)
            if (destroy)
                Destroy(gameObject);
            else
                PlaceAtTarget(entry);
    }

    public bool Moving()
    {
        return !(transform.position == moveTarget.position) || transform.position == exit.position;
    }

    public void MoveToTarget(Transform target, float speedDelta)
    {
        moveTarget = target;
        moveSpeed = speedDelta;
    }

    public void PlaceAtTarget(Transform target)
    {
        moveTarget = target;
        transform.position = target.position;
    }

    public void SetCharacter(DialogueCharacter c)
    {
        character = c;
        characterSprite.sprite = c.characterSprites[0];
    }

    public void SetSprite(Sprite s)
    {
        characterSprite.sprite = s;
    }

    public void HideSprite()
    {
        characterSprite.enabled = false;
    }

    public void ShowSprite()
    {
        characterSprite.enabled = true;
    }
}
