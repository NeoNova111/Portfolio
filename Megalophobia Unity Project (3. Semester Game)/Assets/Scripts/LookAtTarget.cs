using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public float priority = 10;
    public string targetTag = "";

    Transform target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
    }

    private void Update()
    {
        transform.position = target.position;
        //transform.eulerAngles = new Vector3(-target.eulerAngles.x, 0, -target.eulerAngles.z);
        //Debug.Log(transform.eulerAngles);
    }
}
