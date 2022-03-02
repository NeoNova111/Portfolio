using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    private PressurePlatePuzzleManager pppm;
    private bool isPressed = false;
    private Vector3 startPosition;
    private Vector3 pressedPosition;
    // Start is called before the first frame update
    void Start()
    {
        pppm = GetComponentInParent<PressurePlatePuzzleManager>();
        startPosition = transform.position;
        pressedPosition = startPosition;
        pressedPosition.y -= 0.2f;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && isPressed == false)
        //if (other.tag == "Player")
        {
            //isPressed = true;
            pppm.OnChildTriggerEnter(this.gameObject);
            movePressurePlateDown();
        }
    }

    private void movePressurePlateDown()
    {
        transform.position = pressedPosition;
        isPressed = true;
    }

    public void movePressurePlateUp()
    {
        transform.position = startPosition;
        isPressed = false;
    }
}
