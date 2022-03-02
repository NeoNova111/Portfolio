using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;
    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.localPosition;
        //transform.position -= transform.forward;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*transform.position = startPos;
        //transform.position -= transform.forward;
        //transform.LookAt(transform.position + cam.forward);
        transform.localPosition = Vector3.MoveTowards(startPos, cam.position, 5);
        */

        //moves healthbar in front of enemy, rotates it with enemy
        //transform.localPosition = startPos + transform.InverseTransformDirection(transform.forward) *2;

        transform.LookAt(transform.position + cam.forward);
    }
}
