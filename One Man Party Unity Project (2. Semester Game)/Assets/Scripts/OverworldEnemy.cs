using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pathfinding;

public class OverworldEnemy : MonoBehaviour
{

    //all the enemy info
    public int enemyIdxInScene;
    public Race[] race;
    public int enemyLevel;
    public int enemyHealth;
    public int enemyArmor;
    public Job enemyClass;

    public float activationRadius;
    public float pursuitDelay;

    private Transform playerTransform;
    private Vector3 startingPos;
    private bool inPursuit;

    private AIDestinationSetter setter;
    private Transform station;

    public GameEvent BattleEntered;

    bool delayRoutineRunning;
    public AudioSource followSoundSource;
    public AudioSource noticeSoundSource;

    [SerializeField]
    private GameObject noticeExclaimationmark;

    private Coroutine pursuitStop;

    private void Awake()
    {
        if (StaticInfo.enemyDefeatStatus.Count != 0)
        {
            if (StaticInfo.enemyDefeatStatus[enemyIdxInScene])
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        delayRoutineRunning = false;
        inPursuit = false;
        startingPos = transform.position;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        setter = GetComponent<AIDestinationSetter>();
        station = transform.parent;
    }

    private void Update()
    {
        noticeSoundSource.volume = AudioManager.instance.masterVolume;
        followSoundSource.volume = AudioManager.instance.masterVolume;


        if (SeesPlayer())
        {
            if (delayRoutineRunning && pursuitStop != null)
            {
                StopCoroutine(pursuitStop);
                delayRoutineRunning = false;
            }

            setter.target = playerTransform;

            if (!inPursuit)
            {
                StartPursuit();
            }
        }
        else if (inPursuit)
        {
            setter.target = transform;
            if (!delayRoutineRunning)
            {
                pursuitStop = StartCoroutine(StopPursuit(pursuitDelay));
            }
        }
    }

    public void LoadBattle()
    {
        StaticInfo.previousMusic = AudioManager.instance.audioSource.clip;
        StaticInfo.currentOverworldSceneIdx = SceneManager.GetActiveScene().buildIndex;
        StaticInfo.enemyCurrentlyFighting = this;
        StaticInfo.enemyCurrentlyFightingRace = race;

        BattleEntered.Raise();
    }

    void StartPursuit()
    {
        inPursuit = true;
        followSoundSource.Play();
        noticeSoundSource.Play();

        GameObject mark = Instantiate(noticeExclaimationmark, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        Destroy(mark, 0.3f);
    }

    bool SeesPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        Debug.DrawRay(transform.position, direction * activationRadius);

        LayerMask mask = LayerMask.GetMask("Obstacle", "Player", "Pit");
        RaycastHit2D lineOfSight = Physics2D.Raycast(transform.position, direction, activationRadius, mask);

        if (lineOfSight)
        {
            if (lineOfSight.collider.tag == "Player")
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator StopPursuit(float delay) //make stopable
    {
        delayRoutineRunning = true;
        yield return new WaitForSeconds(delay);

        Debug.Log("stopped P");
        inPursuit = false;
        delayRoutineRunning = false;
        setter.target = station;
        followSoundSource.Stop();
    }
}