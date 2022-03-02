using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockworkMovement : MonoBehaviour
{
    [SerializeField] private float StandingStill = 0;
    [SerializeField] private float MovingTime = 0;
    [SerializeField] private float Movement = 0;


    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
       
        if (StandingStill <= 4)
        {
            
            StandingStill += Time.deltaTime;
            
        }
        MovingTime += Time.deltaTime;
        Move();
    }
    void Move()
    {
      
        if (MovingTime < 2)
        {
            transform.Rotate(0, 0, Time.deltaTime * 15, Space.Self);
         
        }
        if (MovingTime > 4){
            MovingTime = 0;
            StandingStill = 0;
        }

    }
}
