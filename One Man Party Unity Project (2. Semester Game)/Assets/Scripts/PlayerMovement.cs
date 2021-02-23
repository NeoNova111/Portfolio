using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 velocityMovement;
    public float movementSpeed;
    private Animator animator;
    private Rigidbody2D rb;
    public BoxCollider2D col;
    public SoundClip footsteps;
    public float footstepDelay = 0.25f;
    private AudioSource playerSounds;

    private void Awake()
    {
        //!!!only works with liniar gameplay!!!
        if (!StaticInfo.gotEnemiesInScenes)
        {
            StaticInfo.enemyDefeatStatus = new List<bool>();
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //adds enemies to the defeatState

            for (int i = 0; i < enemies.Length; i++)
            {
                StaticInfo.enemyDefeatStatus.Add(false);
            }

            StaticInfo.gotEnemiesInScenes = true;
        }
    }

    private void Start()
    {
        playerSounds = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (StaticInfo.loadPlayerInfo)
            LoadPlayerInfo();

        StartCoroutine(playFootstepSounds(footstepDelay));
    }

    void Update()
    {
        velocityMovement = rb.velocity;
        velocityMovement.x = Input.GetAxisRaw("Horizontal");
        velocityMovement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("velx", velocityMovement.x);
        animator.SetFloat("vely", velocityMovement.y);

        rb.velocity = velocityMovement.normalized * movementSpeed * Time.fixedDeltaTime;

        if (rb.velocity.magnitude > movementSpeed)
        {
            rb.velocity *= movementSpeed / rb.velocity.magnitude;
        }

        //StaticInfo.playerPos = transform.position;

        if (velocityMovement != Vector2.zero)
            animator.SetBool("moving", true);
        else
            animator.SetBool("moving", false);
    }

    IEnumerator playFootstepSounds(float delay)
    {
        while (true)
        {
            if (rb.velocity != Vector2.zero)
                footsteps.PlayClip(playerSounds);

            yield return new WaitForSeconds(delay);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            StaticInfo.playerPos = transform.position;
            StaticInfo.loadPlayerInfo = true;
            collision.GetComponent<OverworldEnemy>().LoadBattle();
        }
    }

    void LoadPlayerInfo()
    {
        transform.position = StaticInfo.playerPos;
        StaticInfo.loadPlayerInfo = false;
    }

    public void FreezeMovement()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void UnfreezeMovement()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void DeactivatePlayer()
    {
        //FreezeMovement();
        col.enabled = false;
    }

    public void ReactivatePlayer()
    {
        UnfreezeMovement();
        col.enabled = true;
    }
}