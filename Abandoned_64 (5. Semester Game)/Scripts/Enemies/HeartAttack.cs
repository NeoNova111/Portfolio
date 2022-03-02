using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartAttack : MonoBehaviour
{
    Transform[] parts;
    bool[] active;
    public float damageValue = 1;
    [SerializeField] private float expansionSpeed = 7f;
    [SerializeField] private float attackInterval = 10f;
    [SerializeField] private bool isAttacking = true;
    private float iteration = 1;
    private float timer = 0;
    private bool didDamage = false;
    [SerializeField] private GameEvent heartBeatAttack;
    [SerializeField] private MeshRenderer heartMesh;
    private Material heartMaterialInstance;
    [SerializeField] private GameEvent Anticipation;

    // Start is called before the first frame update
    void Start()
    {
        parts = new Transform[gameObject.transform.childCount];
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = gameObject.transform.GetChild(i).GetComponent<Transform>();
        }

        active = new bool[parts.Length];
        for (int i = 0; i < active.Length; i++)
        {
            active[i] = true;
        }
        heartMaterialInstance = heartMesh.material;
        heartMesh.material = heartMaterialInstance;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking)
        {
            iteration += Time.deltaTime * expansionSpeed;
            for (int i = 0; i < parts.Length; i++)
            {
                if (active[i])
                {
                    parts[i].localScale *= 1 + (expansionSpeed * Time.deltaTime) / iteration;
                }
            }

            timer += Time.deltaTime;
            if(timer == 0) { timer = 0.001f; }
            heartMaterialInstance.SetFloat("Vector1_NextAttack", timer/attackInterval);

            if (timer > attackInterval - 3&&timer<attackInterval - 2)
            {
                Debug.Log("Anticipation started");
                Anticipation.Raise();
            }
            if (timer > attackInterval)
            {
                timer = 0;
                Attack();
            }
        }
        
    }

    public void Attack()
    {
        iteration = 1;
        didDamage = false;
        for (int i = 0; i < active.Length; i++)
        {
            if (active[i])
            {
                parts[i].localScale = new Vector3(1, 1, 1);
            }
            active[i] = true;
        }
        heartBeatAttack.Raise();
    }

    public void toggleIsAttacking (bool activateAttacking)
    {
        isAttacking = activateAttacking;
    }

    public void stun()
    {
        timer -= 1;
        if(timer < 0) { timer = 0; }
    }

    public void doDamage(Collider other)
    {
        if (!didDamage)
        {
            didDamage = true;
            IDamagable damagable = other.GetComponent<IDamagable>();
            if (damagable != null && damagable.Damagable)
            {
                damagable.TakeDamage(damageValue);
            }
        }
    }

    public void OnChildTriggerEnter(GameObject heartPart)
    {
        int index = GetIndexOfHeartPart(heartPart);
        active[index] = false;
        parts[index].localScale = new Vector3(1, 1, 1);
    }

    private int GetIndexOfHeartPart(GameObject heartPart)
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].gameObject == heartPart)
            {
                return i;
            }
        }
        Debug.Log("oh no error :(((");
        return 42069;
    }

}
