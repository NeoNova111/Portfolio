using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseFloor : MonoBehaviour
{
    [SerializeField] private float yOffset = -3;
    [SerializeField] private float animationSpeed = 1f;
    [SerializeField] private GameEvent trigger;
    [SerializeField] private List<MovingGroundEnemy> enemiesToDefeat = new List<MovingGroundEnemy>();

    private Vector3 startPosition;
    private bool floorIsRaising = false;

    private void Start()
    {
        startPosition = gameObject.transform.position;
        foreach(MovingGroundEnemy enemy in enemiesToDefeat)
        {
            enemy.OnDeath += OnEnemyDeath;
        }
    }

    private void OnEnemyDeath(MovingGroundEnemy enemy)
    {
        enemiesToDefeat.Remove(enemy);

        if(enemiesToDefeat.Count == 0)
        {
            StartRaisingFloor();
        }
    }

    private void Update()
    {
        if (floorIsRaising)
        {
            if (yOffset <= 0)
            {
                if (gameObject.transform.position.y >= startPosition.y + yOffset)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y -animationSpeed*Time.deltaTime, gameObject.transform.position.z);
                }
                else
                {
                    gameObject.transform.position = new Vector3(startPosition.x, startPosition.y + yOffset, startPosition.z);
                    floorIsRaising = false;
                }
            }
            else
            {
                if (gameObject.transform.position.y <= startPosition.y + yOffset)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + animationSpeed * Time.deltaTime, gameObject.transform.position.z);
                }
                else
                {
                    gameObject.transform.position = new Vector3(startPosition.x, startPosition.y + yOffset, startPosition.z);
                    floorIsRaising = false;
                }
            }
            
        }
    }

    public void StartRaisingFloor()
    {
        floorIsRaising = true;
    }
}
