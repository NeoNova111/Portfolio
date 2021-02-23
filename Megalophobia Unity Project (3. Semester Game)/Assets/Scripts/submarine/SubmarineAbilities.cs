using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineAbilities : MonoBehaviour
{
    public bool enableFlare;
    public GameObject falre;
    public Vector3 offset;
    public int flareAmount;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && enableFlare)
        {
            DropFlare();
        }
    }

    void DropFlare()
    {
        if (flareAmount <= 0)
            return;

        GameObject flare = Instantiate(falre, transform.position + offset, Quaternion.identity);
        flare.GetComponent<Rigidbody>().AddTorque(Vector3.right);
        flareAmount--;
    }
}
