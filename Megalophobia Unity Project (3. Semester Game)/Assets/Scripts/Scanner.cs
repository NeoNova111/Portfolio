using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public Transform scanSphere;
    public float scanSpeed;
    bool scanning;

    private void Start()
    {
        scanning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            scanSphere.localScale = new Vector3(1, 1, 1);
            scanning = true;
        }

        if (scanning)
        {
            scanSphere.localScale += scanSphere.localScale * (scanSpeed * Time.deltaTime);
            if(scanSphere.localScale.x > 10000)
            {
                scanning = false;
            }
        }
    }
}
