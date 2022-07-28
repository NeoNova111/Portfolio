using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability")]
public class Ability : ScriptableObject
{
    public Sprite icon;
    public int maxUseCount;
    private int currentUses;
    public int CurrentUses { get => currentUses; set => currentUses = value; }
    public float cooldown;
    private float currentCooldown;
    public float CurrentCooldown { get => currentCooldown; set => currentCooldown = value; }
    public bool Usable { get => currentUses > 0; }
    [HideInInspector] public bool usedThisFrame = false;

    public void UpdateUse()
    {
        if (currentUses == 0) return;

        usedThisFrame = true;
        currentUses--;
    }
}
