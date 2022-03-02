using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMovement : MonoBehaviour, ICollectable, IPullable
{
    [SerializeField] private bool collectable = true;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameEvent collectedEvent;
    [SerializeField] private ParticleSystem Particles;
    [SerializeField] private BoxCollider Collision;

    [SerializeField] private float rotationSpeed = 2;
    [SerializeField] private float verticalSpeed = 2;
    [SerializeField] private float height = 1;

    private PlayerStateMachine player;
    private bool pullable = true;
    public bool Pullable { get => pullable; }
    [SerializeField] private float ups = 1.5f;



    private Vector3 startPos;

    public bool Collectable { get => collectable; }

    private void Start()
    {
        startPos = transform.position;

        player = PlayerStateMachine.Instance;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        transform.position = startPos + new Vector3(0.0f, Mathf.Sin(Time.time * verticalSpeed) * height, 0.0f);
    }

    public void Pull()
    {
        startPos = Vector3.MoveTowards(startPos, player.transform.position + Vector3.up, ups * Time.deltaTime);
        if(transform.position == player.transform.position)
        {
            Collect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        playerStats.collectibleCount++;
        collectedEvent.Raise();
        Particles.Play();
        this.transform.localScale = new Vector3(0, 0, 0);
        StartCoroutine(Collecteffects());
        Collision.enabled = false;

    }
    IEnumerator Collecteffects()
    {
        
        yield return new WaitForSeconds(0.8f);
        Destroy(transform.parent.gameObject);
    }
}
