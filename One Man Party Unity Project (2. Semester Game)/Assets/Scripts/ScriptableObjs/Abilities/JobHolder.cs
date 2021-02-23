using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JobHolder
{
    public JobHolder(Job job)
    {
        jobHolding = job;
    }

    public Job jobHolding;
    public int cooldownOne;
    public int cooldownTwo;

    public void Start()
    {
        Setup();
    }

    public void Setup()
    {
        cooldownOne = 0;
        cooldownOne = 0;
    }

    public void UseAbilityOne(Unit target, Unit user)
    {
        jobHolding.abilityOne.UseAbility(target, user);
    }

    public void UseAbilityTwo(Unit target, Unit user)
    {
        jobHolding.abilityTwo.UseAbility(target, user);
    }

    public void ResetCD()
    {
        cooldownOne = 0;
        cooldownTwo = 0;
    }

    public void MaxCDS()
    {
        MaxCDOne();
        MaxCDTwo();
    }

    public void MaxCDOne()
    {
        cooldownOne = jobHolding.abilityOne.cooldown;
    }

    public void MaxCDTwo()
    {
        cooldownTwo = jobHolding.abilityTwo.cooldown;
    }

    public void CountDownCDS()
    {
        if (cooldownOne > 0)
            cooldownOne--;

        if (cooldownTwo > 0)
            cooldownTwo--;
    }

    //public int GetCD()
    //{
    //    return coolDown;
    //}

    //public int GetCT()
    //{
    //    return castTime;
    //}

    //public void SetCD(int cd)
    //{
    //    coolDown = cd;
    //}

    //public void SetCT(int ct)
    //{
    //    castTime = ct;
    //}
}
