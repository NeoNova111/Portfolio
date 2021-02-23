using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeThisObjectLookAt : MonoBehaviour
{
    [SerializeField] string tagToLookAt;
    [SerializeField] Transform objectToLookAt;

    Transform lookAtTarget;

    [SerializeField] bool ignoreX = false;
    [SerializeField] bool ignoreY = false;
    [SerializeField] bool ignoreZ = false;

    void Start()
    {
        if (objectToLookAt)
        {
            lookAtTarget = objectToLookAt;
        }
        else if (GameObject.FindGameObjectWithTag(tagToLookAt))
        {
            lookAtTarget = GameObject.FindGameObjectWithTag(tagToLookAt).transform;
        }
    }

    void Update()
    {
        if (!lookAtTarget)
        {
            Debug.LogWarning(gameObject.name + " has nothing to look at despite owning the MakeThisObjectLookAt component");
            return;
        }

        Vector3 eulerRot = transform.eulerAngles;
        transform.LookAt(lookAtTarget);

        if (ignoreX)
            transform.eulerAngles = new Vector3(eulerRot.x, transform.eulerAngles.y, transform.eulerAngles.z);

        if (ignoreY)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, eulerRot.y, transform.eulerAngles.z);

        if(ignoreZ)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, eulerRot.z);
    }
}
