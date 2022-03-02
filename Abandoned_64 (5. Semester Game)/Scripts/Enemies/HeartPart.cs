using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPart : MonoBehaviour
{
    private HeartAttack heartAttack;
    // Start is called before the first frame update
    void Start()
    {
        heartAttack = GetComponentInParent<HeartAttack>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "HearthBeatPart" && other.tag != "Hitbox")
        {
            //gameObject.SetActive(false);
            heartAttack.OnChildTriggerEnter(gameObject);
            if(other.tag == "Player")
            {
                heartAttack.doDamage(other);
            }
        }
    }
}
