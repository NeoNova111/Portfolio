using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardCondition : MonoBehaviour
{
    private bool mayor = false;
    private bool sculptor = false;
    private bool researcher = false;
    [SerializeField] private GameEvent letPass;

    public void TalkedToMayor()
    {
        mayor = true;
        CheckCcondition();
    }

    public void TalkedToSculptor()
    {
        sculptor = true;
        CheckCcondition();
    }

    public void TalkedToResearcher()
    {
        researcher = true;
        CheckCcondition();
    }

    public void CheckCcondition()
    {
        if(mayor && sculptor && researcher)
        {
            letPass.Raise();
        }
    }
}
