using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FalloffType { Gradual, Instant, CustomCurve }
public enum BuffType { Overhealth, Shield, Defense, Damage, Jump, Dash, WalkSpeed, CappedSpeed, MaxSpeed, CooldownReduction }

[System.Serializable]
public class TempBuff //class instead of struct for nullability
{
    public BuffType buffType;
    public FalloffType falloffType;
    public float increaseValue;
    public float duration;
}

[System.Serializable]
public class PermaBuff //class instead of struct for nullability
{
    [System.Serializable]
    public struct Buff
    {
        public BuffType buffType;
        public float increaseValue;
    }

    public Sprite characterHeadVisual;
    public Buff[] buffs;
}

[System.Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private float flatIncrease;
    public float FlatIncrease { get => flatIncrease; set => flatIncrease = value; }
    public float TotalValue { get => buff == null ? baseValue + flatIncrease : baseValue + flatIncrease + buff.increaseValue; }

    [HideInInspector] public TempBuff buff;
    private float remainingBuffDuration;
    private float startIncreaseValue;

    public void Update()
    {
        //do all the buff stuff
        if (buff == null || remainingBuffDuration == 0) return; //check if duration == 0 before lowering duration for proper exectution

        remainingBuffDuration = Mathf.Clamp(remainingBuffDuration - Time.deltaTime, 0, remainingBuffDuration);
        switch (buff.falloffType)
        {
            case FalloffType.Gradual:
                buff.increaseValue = Mathf.Lerp(0, startIncreaseValue, remainingBuffDuration / buff.duration);
                if (remainingBuffDuration == 0)
                {
                    UIManager.Instance.BuffTimerHolder.RemoveBuffFromDisplay(buff.buffType);
                    buff = null;
                }
                break;
            case FalloffType.Instant:
                if (remainingBuffDuration == 0)
                {
                    UIManager.Instance.BuffTimerHolder.RemoveBuffFromDisplay(buff.buffType);
                    buff = null; //remove buff once it runs out
                }
                break;
            case FalloffType.CustomCurve:
                if (remainingBuffDuration == 0)
                {
                    UIManager.Instance.BuffTimerHolder.RemoveBuffFromDisplay(buff.buffType);
                    buff = null; //remove buff once it runs out
                }
                break;
            default:
                break;
        }
    }

    public void SetBuff(TempBuff buff)
    {
        //rn the newest buff overrides buffs of the same type to lazily prevent stacking and gamebreaking stat increases
        this.buff = buff;
        remainingBuffDuration = buff.duration;
        startIncreaseValue = buff.increaseValue;
        UIManager.Instance.BuffTimerHolder.AddBuffToDisplay(buff);
    }

    public void Awake()
    {
        //reset stuff on awake
        remainingBuffDuration = 0f;
        flatIncrease = 0f;
    }
}

[CreateAssetMenu(menuName = "PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public List<PermaBuff> permanentBuffs;

    [Header("Health")]
    public Stat healthStat;
    public float CurrentHealth = 100f;

    public Stat shieldStat;
    public float CurrentShield = 0f;
    public float ShieldRegenatationDelay = 3f;
    private float currentShieldRegenerationDelay;
    public float ShieldRegenerationRate = 10f;

    [Header("Speed")]
    public Stat walkSpeedStat;
    public Stat cappedHorizontalSpeedStat;
    public Stat maxHorizontalSpeedStat;

    //dash
    public Stat dashStat;

    [Header("Jump")]
    public Stat jumpStat;


    [Header("Damage")]
    public Stat damageStat;
    public float DamageMultiplier = 1f;

    [Header("Defensive")]
    public Stat defenseStat;
    public int ArmorStacks = 0; //negates damage once for each stack

    [Header("Cooldowns")]
    public Stat cooldownReduction;

    public GameEvent HealthChange;

    public AK.Wwise.RTPC PlayerHealthRTCP;

    public void Update()
    {
        RegenerateShield();
        healthStat.Update();
        shieldStat.Update();
        walkSpeedStat.Update();
        cappedHorizontalSpeedStat.Update();
        maxHorizontalSpeedStat.Update();
        dashStat.Update();
        jumpStat.Update();
        defenseStat.Update();
        cooldownReduction.Update();
        damageStat.Update();
    }

    public void FakeAwake()
    {
        permanentBuffs.Clear();

        //setup
        healthStat.Awake();
        shieldStat.Awake();
        walkSpeedStat.Awake();
        cappedHorizontalSpeedStat.Awake();
        maxHorizontalSpeedStat.Awake();
        dashStat.Awake();
        jumpStat.Awake();
        defenseStat.Awake();
        cooldownReduction.Awake();
        damageStat.Awake();

        CurrentShield = 0;
        currentShieldRegenerationDelay = ShieldRegenatationDelay;
        Heal(1000f); //start with full health (also raises health change event, so it saves a line and I'm lazy)
        PlayerHealthRTCP.SetGlobalValue((CurrentHealth / healthStat.TotalValue) * 100); //wwise health value
    }

    //for some reason I forgot dictionaries exist here apparently, but it's fuíne for now so I'm keeping it
    public Stat GetCorrispondingStatToBuffType(BuffType type)
    {
        switch (type)
        {
            case BuffType.Overhealth:
                return healthStat;
            case BuffType.Damage:
                return damageStat;
            case BuffType.Defense:
                return defenseStat;
            case BuffType.WalkSpeed:
                return walkSpeedStat;
            case BuffType.CappedSpeed:
                return cappedHorizontalSpeedStat;
            case BuffType.MaxSpeed:
                return maxHorizontalSpeedStat;
            case BuffType.Jump:
                return jumpStat;
            case BuffType.Dash:
                return dashStat;
            case BuffType.Shield:
                return shieldStat;
            case BuffType.CooldownReduction:
                return cooldownReduction;
            default:
                break;
        }

        return null;
    }

    public void CalculateDamageTaken(float damage, bool directDamage = false)
    {
        float remainingDamage;
        if (!directDamage)
        {
            if(ArmorStacks > 0)
            {
                ArmorStacks--;
                HealthChange.Raise();
                return;
            }

            remainingDamage = damage - defenseStat.TotalValue; //flat damage reduction due to defense
            if(remainingDamage >= CurrentShield)
            {
                remainingDamage = remainingDamage - CurrentShield;
                CurrentShield = 0;
            }
            else
            {
                CurrentShield -= remainingDamage;
                remainingDamage = 0;
            }
        }
        else
        {
            remainingDamage = damage;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth - remainingDamage, 0, healthStat.TotalValue);

        float remainingHealthPercentage = CurrentHealth / healthStat.TotalValue;
        PlayerHealthRTCP.SetGlobalValue(remainingHealthPercentage * 100); //wwise health value
        if (PostProcessingControl.Instance)
        {
            PostProcessingControl.Instance.VignettePulse(remainingHealthPercentage, Color.red);
        }

        AkSoundEngine.PostEvent("Player_Hit", PlayerController.Instance.gameObject);
        HealthChange.Raise();
        currentShieldRegenerationDelay = ShieldRegenatationDelay;
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, healthStat.TotalValue);
        if (PostProcessingControl.Instance)
        {
            PostProcessingControl.Instance.VignettePulse(CurrentHealth / healthStat.TotalValue, Color.green);
        }
        HealthChange.Raise();
    }

    public void AddPermanentBuff(PermaBuff permaBuff)
    {
        UIManager.Instance.PlayerHeadUI.AddItemVisual(permaBuff.characterHeadVisual);
        permanentBuffs.Add(permaBuff);

        foreach(var buff in permaBuff.buffs)
        {
            GetCorrispondingStatToBuffType(buff.buffType).FlatIncrease += buff.increaseValue;
            //if (buff.buffType == BuffType.Overhealth)
            //{
            //    Heal(0f); //just to update
            //}
        }
    }

    public void RemovePermanentBuff(PermaBuff buffToRemove)
    {
        UIManager.Instance.PlayerHeadUI.RemoveItemVisual(buffToRemove.characterHeadVisual);
        permanentBuffs.Remove(buffToRemove);

        foreach (var buff in buffToRemove.buffs)
        {
            GetCorrispondingStatToBuffType(buff.buffType).FlatIncrease -= buff.increaseValue;
        }
    }

    public void RemovePermanentBuff(int atIndex)
    {
        if (atIndex >= permanentBuffs.Count) return;
        permanentBuffs.RemoveAt(atIndex);
    }

    private void RegenerateShield()
    {
        if (currentShieldRegenerationDelay > 0) currentShieldRegenerationDelay = Mathf.Clamp(currentShieldRegenerationDelay - Time.deltaTime, 0, ShieldRegenatationDelay);

        if (currentShieldRegenerationDelay <= 0)
        {
            CurrentShield = Mathf.Clamp(CurrentShield + ShieldRegenerationRate * Time.deltaTime, 0, shieldStat.TotalValue);
            HealthChange.Raise();
        }
    }
}
