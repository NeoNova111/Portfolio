using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTeamHeads : MonoBehaviour
{
    private Transform[] heads;
    public Vector3 lookAtOffset = new Vector3(0, 0, -100);
    //public Vector3[] startRotations;
    public Vector3 maxRotationsOffset;
    public Vector3 offsetSpeed;
    public float rotationSpeed;
    private Vector3[] offset;
    // Start is called before the first frame update
    void Start()
    {
        heads = new Transform[gameObject.transform.childCount];
        offset = new Vector3[heads.Length];
        for (int i = 0; i < heads.Length; i++)
        {
            heads[i] = gameObject.transform.GetChild(i).GetComponent<Transform>();
        }
        //startRotations = new Vector3[heads.Length];
        
        
        //for (int k = 0; k<heads.Length; k++)
        //{
        //    startRotations[k] = heads[k].transform.eulerAngles;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < heads.Length; i++)
        {
            offset[i] += new Vector3(Random.Range(-offsetSpeed.x, offsetSpeed.x), Random.Range(-offsetSpeed.y, offsetSpeed.y), Random.Range(-offsetSpeed.z, offsetSpeed.z));
            if (offset[i].x < -maxRotationsOffset.x){offset[i].x = -maxRotationsOffset.x;}
            if (offset[i].x > maxRotationsOffset.x) { offset[i].x = maxRotationsOffset.x; }
            if (offset[i].y < -maxRotationsOffset.y) { offset[i].y = -maxRotationsOffset.y; }
            if (offset[i].y > maxRotationsOffset.y) { offset[i].y = maxRotationsOffset.y; }
            if (offset[i].z < -maxRotationsOffset.z) { offset[i].z = -maxRotationsOffset.z; }
            if (offset[i].z > maxRotationsOffset.z) { offset[i].z = maxRotationsOffset.z; }
            //heads[i].transform.eulerAngles = Vector3.RotateTowards(heads[i].transform.eulerAngles, startRotations[i] + offset[i], rotationSpeed, rotationSpeed);
            //heads[i].transform.eulerAngles = Vector3.Lerp(heads[i].transform.eulerAngles, startRotations[i] + offset[i], rotationSpeed);
            //heads[i].transform.rotation = Quaternion.Lerp(heads[i].transform.rotation, Quaternion.Euler(offset[i].x, offset[i].y, offset[i].z), rotationSpeed);
            Quaternion oldRot = heads[i].rotation;
            heads[i].transform.LookAt(heads[i].position + lookAtOffset + offset[i]);
            heads[i].transform.rotation = Quaternion.Lerp(oldRot, heads[i].rotation, rotationSpeed);
        }
    }
}
