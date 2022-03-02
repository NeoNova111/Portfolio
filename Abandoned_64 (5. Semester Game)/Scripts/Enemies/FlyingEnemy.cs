using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [SerializeField] private Transform[] Waypoints;
    //public Transform enemyTransform;
    // Start is called before the first frame update
    private int numOfWaypoints = 0;
    [SerializeField] private int NextWaypoint;

    [SerializeField] private float flightSpeed = 5;
    [SerializeField] private float maxDist;

    [SerializeField] private bool regenerateHealth = true;
    [SerializeField] private float healthRegenerationPerSecond = 100f;

    private new void Start()
    {
        base.Start();
        numOfWaypoints = Waypoints.Length;
        float minDist = 10000;
        for (int i = 0; i < numOfWaypoints; i++)
        {
            Transform currentT = Waypoints[i];
            float CurrentDist = (currentT.position - transform.position).sqrMagnitude;
            if (CurrentDist < minDist)
            {
                minDist = CurrentDist;
                NextWaypoint = (i + 1) % numOfWaypoints;
            }
        }
    }

   

    // Update is called once per frame
    private void Update()
    {
        //Debug.Log("asd   " + (DragonEnemy.position - Waypoints[NextWaypoint].position).magnitude );
        if ((transform.position - Waypoints[NextWaypoint].position).magnitude < maxDist)
        {
            NextWaypoint = (NextWaypoint + 1) % numOfWaypoints;
        }
        Vector3 newPos = Vector3.MoveTowards(transform.position, Waypoints[NextWaypoint].position, flightSpeed * Time.deltaTime);

        transform.position = newPos;

        if (regenerateHealth && currentHealth < startHealth)
            Regenerate(healthRegenerationPerSecond);
    }
}
