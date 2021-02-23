using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//[CreateAssetMenu(menuName = "Unit")]
public class Unit : MonoBehaviour
{
    public BackpackInventory playerClasses;

    public Race race;
    public int level;
    public int health;
    public int maxHealth;
    public int armor;
    public Job job; //equiped Job
    public JobHolder[] jobs;
    public int currentJobIdx;

    private float turnPos;

    public StatusEffect statusEffect;
    public int effectDuration;

    public float attackMultiplier;
    public float defenseMultiplier;
    public float resistanceMultiplier;
    public float speedMultiplier;

    public GameEvent damage;
    public Shaker shaker;

    public GameObject dmgNumberPrefab;

    private SpriteRenderer unitSpriteRenderer;

    private void Awake()
    {
        attackMultiplier = 1;
        defenseMultiplier = 1;
        speedMultiplier = 1;
        resistanceMultiplier = 1;

        currentJobIdx = 0;

        SetJobs();

        if(job != null)
            turnPos = job.speed;
    }

    private void Start()
    {
        unitSpriteRenderer = GetComponent<SpriteRenderer>();
        shaker = GetComponent<Shaker>();

        MulAllMultiplier(race.statMultiplier);
    }

    public void SetStatusEffect(StatusEffect status)
    {
        if (statusEffect == null)
        {
            statusEffect = status;
            effectDuration = status.turnDuration;
            unitSpriteRenderer.color = status.color;
        }
    }

    public void ClearStatusEffect()
    {
        statusEffect = null;
        effectDuration = 0;
        unitSpriteRenderer.color = new Color(255, 255, 255);
    }

    public bool AllAbilitiesOnCooldown()
    {
        foreach(JobHolder job in jobs)
        {
            if (job.cooldownOne == 0 || job.cooldownTwo == 0)
                return false;
        }
        return true;
    }

    public bool EquipedJobAbilitiesOnCooldown()
    {
        if(jobs[currentJobIdx].cooldownOne == 0 || jobs[currentJobIdx].cooldownOne == 0)
            return false;

        return true;
    }

    public void HealUnit(int healValue)
    {
        health += healValue;
        if (health > maxHealth) health = maxHealth;

        GameObject dmg = Instantiate(dmgNumberPrefab, gameObject.transform.position + new Vector3(0, 0, 1), Quaternion.identity);
        dmg.GetComponent<DmgNumber>().SetDmgText(healValue, DmgColor.HEAL);
    }

    public bool TakeDamage(int damageValue)
    {
        if (damageValue <= 0)
            damageValue = 1;

        health -= damageValue;
        if (health <= 0)
            health = 0;

        damage.Raise();
        if(shaker != null)
            shaker.StartShake();

        GameObject dmg = Instantiate(dmgNumberPrefab, gameObject.transform.position + new Vector3(0, 0, 1), Quaternion.identity);
        dmg.GetComponent<DmgNumber>().SetDmgText(damageValue, DmgColor.HEALTH);

        return isDead();
    }

    public int TakeArmorDamage(int damageValue)
    {
        int remainingDmg;
        remainingDmg = damageValue - armor;
        if(remainingDmg > 0)
        {
            GameObject dmg = Instantiate(dmgNumberPrefab, gameObject.transform.position + new Vector3(0, 0, 1), Quaternion.identity);
            dmg.GetComponent<DmgNumber>().SetDmgText(armor, DmgColor.ARMOR);
            armor = 0;
        }
        else
        {
            GameObject dmg = Instantiate(dmgNumberPrefab, gameObject.transform.position + new Vector3(0, 0, 1), Quaternion.identity);
            dmg.GetComponent<DmgNumber>().SetDmgText(damageValue, DmgColor.ARMOR);
            armor = armor - damageValue;
            remainingDmg = 0;
        }
        return remainingDmg;
    }

    public void UpdateValues()
    {
        //count buffs and update values
    }

    public bool UseAbilityOne(Unit target)
    {
        if (jobs[currentJobIdx].cooldownOne == 0)
        {
            jobs[currentJobIdx].MaxCDOne();
            job.abilityOne.UseAbility(target, this);
            return true;
        }
        return false;
    }

    public bool UseAbilityTwo(Unit target)
    {
        if (jobs[currentJobIdx].cooldownTwo == 0)
        {
            jobs[currentJobIdx].MaxCDTwo();
            job.abilityTwo.UseAbility(target, this);
            return true;
        }
        return false;
    }

    public void DecreaseCooldowns()
    {
        foreach (JobHolder j in jobs)
        {
            j.CountDownCDS();
        }
    }

    public void DecreaseEffectDuration()
    {
        effectDuration -= 1;
        if (effectDuration <= 0)
        {
            ClearStatusEffect();
        }
    }

    public bool isDead()
    {
        if (health <= 0)
        {
            return true;
        }
        return false;
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    public void SetJobs()
    {
        if (gameObject.tag == "Player") // for player with jobs in backpackinventory
        {
            jobs = new JobHolder[playerClasses.classInventory.Length];
            for (int i = 0; i < jobs.Length; i++)
            {
                jobs[i] = new JobHolder(playerClasses.classInventory[i]);
            }
        }
        else // for enemies with only one job
        {
            jobs = new JobHolder[1];
            jobs[0] = new JobHolder(job);
        }
    }

    public float GetTurnPos()
    {
        return turnPos;
    }

    public void SetTurnPos(float val)
    {
        turnPos = val;
    }

    public void AddToTurnPos(float val)
    {
        turnPos += val;
    }

    public void OnMouseEnter()
    {
        BattleSystem.instance.TargetEnemy(this);
    }

    #region StatMultiplier
    public float GetAttackMultiplier() { return attackMultiplier; }
    public void SetAttackMultiplier(float val) { attackMultiplier = val; }
    public void MulAttackMultiplier(float val) { attackMultiplier *= val; }

    public float GetDefenseMultiplier() { return defenseMultiplier; }
    public void SetDefenseMultiplier(float val) { defenseMultiplier = val; }
    public void MulDefenseMultiplier(float val) { defenseMultiplier *= val; }

    public float GetResistanceMultiplier() { return resistanceMultiplier; }
    public void SetResistanceMultiplier(float val) { resistanceMultiplier = val; }
    public void MulResistanceMultiplier(float val) { resistanceMultiplier *= val; }

    public void SetAllMultiplier(float val)
    {
        SetAttackMultiplier(val);
        SetDefenseMultiplier(val);
        SetResistanceMultiplier(val);
    }

    public void MulAllMultiplier(float val)
    {
        MulAttackMultiplier(val);
        MulDefenseMultiplier(val);
        MulResistanceMultiplier(val);
    }
    #endregion
}