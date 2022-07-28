using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SImpleBillboard : MonoBehaviour
{
    public bool onlyYRotation = true;
    private Transform targetTransform;

    void Start()
    {
        targetTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (onlyYRotation) transform.LookAt(new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z), Vector3.up);
        else transform.LookAt(targetTransform.position, Vector3.up);
    }
}
