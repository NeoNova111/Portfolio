using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ITargetable, IDamagable
{
    [Header("Super Enemy Variables")]
    [SerializeField] protected float startHealth = 30;
    public float currentHealth;
    [SerializeField] protected ParticleSystem hitEffect;
    [SerializeField] protected GameEvent enemyHitEvent;
    [SerializeField] protected GameEvent enemyAttack;
    [SerializeField] protected GameEvent enemyDeath;

    //targetable interface
    [SerializeField] public bool targetable = true;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private bool addHealtbar = true;
    [SerializeField] private Transform healthBarTransform;


    public bool Targetable { get => targetable; }
    public Transform TargetTransform { get { return targetTransform ? targetTransform : transform; } }

    //damagable interface
    [SerializeField] private bool damagable = true;
    public bool Damagable { get => damagable; }

    private HealthBar healthBar;
    float slowdownFactor = 0;
    float slowdownLength = 0.1f;
    [SerializeField] public SkinnedMeshRenderer meshRenderer;
    public float DamageLightup;
    protected virtual void Start()
    {
        //if(hitEffect)
        //    hitEffect.Simulate(Time.unscaledDeltaTime, true);

        currentHealth = startHealth;
        VisualizeTargetable();
        if (addHealtbar){AddHealthBar();}
    }

    public virtual void TakeDamage(float damage)
    {
        DamageLightup = 1;
        currentHealth -= damage;

        if(enemyHitEvent)
            enemyHitEvent.Raise();

        if(hitEffect)
            hitEffect.Play();

        if(currentHealth <= 0)
        {
            if (enemyDeath)
            {
                enemyDeath.Raise();
            }
            Die();
        }
    }

    public virtual void Attack()
    {
        if(enemyAttack)
            enemyAttack.Raise();
    }

    protected void Regenerate(float regenPerSecond)
    {
        currentHealth = Mathf.Clamp(currentHealth + regenPerSecond * Time.deltaTime, 0, startHealth);
    }

    //other stuff
    protected virtual void OnEnemyDeath()
    {
        CameraController camController = CameraController.Instance;
        if (camController != null)
        {
            if (this.GetComponent<ITargetable>() == camController.LockOnTarget)
            {
                //toggle off and on to lose the now dead enemy but find potential new target
                camController.TryRelock();
            }
        }
    }

    protected void OnDisable()
    {
        OnEnemyDeath();
    }

    protected void OnDestroy()
    {
        OnEnemyDeath();
    }

    protected virtual void Die()
    {
     
        StartCoroutine(DeathCoroutine());
    }

    public void VisualizeTargetable()
    {
        Renderer rend = GetComponent<Renderer>();
        float radius = 0;
        Vector3 position = new Vector3(0, 0, 0);
        if (rend != null)
        {
            //Vector3 center = rend.bounds.center;
            radius = rend.bounds.extents.magnitude;
        }
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            foreach (var r in renderers)
            {
                if (r.bounds.extents.magnitude > radius)
                {
                    radius = r.bounds.extents.magnitude;
                    position = r.transform.localPosition;
                }
            }
        }
        if (radius == 0)
        {
            radius = 2;
        }
        GameObject instance = Instantiate(Resources.Load("Prefab/TargetableVisualizer", typeof(GameObject))) as GameObject;
        instance.transform.localScale = new Vector3(radius, radius, radius);
        instance.transform.parent = gameObject.transform;
        instance.transform.localPosition = position;
    }

    protected void AddHealthBar()
    {
        GameObject instance = Instantiate(Resources.Load("Prefab/CanvasHealthBar", typeof(GameObject))) as GameObject;

        if (healthBarTransform != null)
        {
            instance.transform.parent = healthBarTransform;
            instance.transform.localPosition = new Vector3(0,0,0);
        }
        else
        {
            Renderer rend = GetComponent<Renderer>();
            float radius = 0;
            Vector3 position = new Vector3(0, 0, 0);
            if (rend != null)
            {
                //Vector3 center = rend.bounds.center;
                radius = rend.bounds.extents.magnitude;
            }
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                foreach (var r in renderers)
                {
                    if (r.bounds.extents.magnitude > radius)
                    {
                        radius = r.bounds.extents.magnitude;
                        position = r.transform.localPosition;
                    }
                }
            }
            if (radius == 0){radius = 2;}
            position.y += radius;
            instance.transform.parent = gameObject.transform;
            instance.transform.localPosition = position;
        }
        healthBar = instance.GetComponentInChildren<HealthBar>();
        healthBar.SetEnemy(this);
        healthBar.SetMaxHealth(startHealth);
    }

    protected IEnumerator DeathCoroutine() //why?
    {
       
       
        OnEnemyDeath();
        targetable = false;
        //this.transform.localScale = new Vector3(0, 0, 0);
        //this.transform.position += new Vector3(0, 1.5f, 0);
        yield return new WaitForSecondsRealtime(0.1f); //why?
        //Destroy(gameObject);
     
        gameObject.SetActive(false);
    }
}
