using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JobType {OFFENSE, DEFENSE, SUPPORT }

[CreateAssetMenu(fileName ="New Job", menuName = "Job")]
public class Job : ScriptableObject
{
    public new string name;
    public JobType type = JobType.OFFENSE;
    public Sprite icon = null;
    public int attack; //effects all attacks exept truedamage attacks
    public int defense; //against normal
    public int resistance; //against magic
    [Range(1, 101)]
    public int speed;
    [Range(0f, 1f)]
    public float luck; //affects chances
    public Ability abilityOne;
    public Ability abilityTwo;
}
