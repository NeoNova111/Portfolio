using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFire : MonoBehaviour
{
    private Animator anim;
    [SerializeField] GameObject ChargeForm;
    [SerializeField] ScuffedEnemy enemyScript;
    // Start is called before the first frame update
    void Start()
    {
        ChargeForm.SetActive(false);
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    void ChargeUp()
    {
        ChargeForm.SetActive(true);
        Debug.Log("Charging");
    }

    void ChargeFinished()
    {
        ChargeForm.SetActive(false);
        Debug.Log("ChargeFinished");
    }

    void ProjectileFired()
    {
        enemyScript.Attack();
        anim.SetBool("IsAttacking", false);
    }

    void OutOfStun()
    {
        enemyScript.stunned = false;
        anim.SetBool("IsHit1", false);
    }
}
