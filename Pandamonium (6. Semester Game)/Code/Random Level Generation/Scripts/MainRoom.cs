using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRoom : MonoBehaviour
{
    public Door[] doors;
    public Transform[] DoorTransforms { get => doorTransforms; }
    public Collider IndentCollider { get => indentCollider; }

    [SerializeField] private GameObject[] enemySpawnPatterns;
    [SerializeField] private Collider pushCollider;
    [SerializeField] private Collider indentCollider;
    [SerializeField] private Collider enterCollider;


    private EnemyHierarchicalStateMachine[] enemies = new EnemyHierarchicalStateMachine[0];

    private Transform[] doorTransforms;
    private bool cleared = false;
    private bool generated = false;

    private void Awake()
    {
        doorTransforms = new Transform[doors.Length];
        for(int i = 0; i < doorTransforms.Length; i++)
        {
            doorTransforms[i] = doors[i].Transform;
        }
    }

    private void Start()
    {
        indentCollider.gameObject.SetActive(false);
        pushCollider.gameObject.SetActive(false);
        enterCollider.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (cleared) return;

        if (CheckRoomIfCleared())
        {
            RoomCleared();
        }
    }

    public void EnterRoom()
    {
        foreach(var door in doors)
        {
            door.CloseDoor();
        }

        foreach (var enemy in enemies)
        {
            enemy.Passive = false;
        }
    }

    private void SpawnEnemies()
    {
        int rnd = Random.Range(0, enemySpawnPatterns.Length);
        enemySpawnPatterns[rnd].SetActive(true);

        enemies = enemySpawnPatterns[rnd].GetComponentsInChildren<EnemyHierarchicalStateMachine>();
        foreach(var enemy in enemies)
        {
            enemy.Passive = true;
        }

        foreach (var enemy in enemies)
        {
            enemy.Passive = true;
            enemy.transform.parent = null; //so the enemies can detect collisions on them, otherwise their parent rigidbody of the rooms will do it for them (alternatively we could give the enemies their own rigidbody)
        }
    }

    private void RoomCleared()
    {
        cleared = true;
        enterCollider.enabled = false;
        foreach(var door in doors)
        {
            if (door.IsEntrance) door.OpenDoor();
        }
    }

    private bool CheckRoomIfCleared()
    {
        if (!generated) return false;

        bool cleared = true;
        foreach(var enemy in enemies)
        {
            if(enemy != null)
            {
                cleared = false;
                break;
            }
        }
        return cleared;
    }

    public void RemoveNormalRoomComponent()
    {
        NormalRoom normal = GetComponent<NormalRoom>();
        if (normal) Destroy(normal);
    }

    public void SeperatedRoomState()
    {
        SpawnEnemies();
        indentCollider.gameObject.SetActive(false);
        pushCollider.gameObject.SetActive(false);
        enterCollider.gameObject.SetActive(true);

        generated = true;
    }

    public void PrepForGeneration()
    {
        generated = false;
        indentCollider.gameObject.SetActive(true);
        pushCollider.gameObject.SetActive(true);
        enterCollider.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>()) EnterRoom();
    }

    private void OnDisable()
    {
        foreach(var e in enemies)
        {
            Destroy(e.gameObject);
        }
    }

    private void OnEnable()
    {
        enemies = new EnemyHierarchicalStateMachine[0];
    }
}
