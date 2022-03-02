using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveGameObject : MonoBehaviour
{
    public GameObject objectToActivate;

    //might look weird, but animation events do not take params
    public void SetActive(bool active)
    {
        if (objectToActivate)
            objectToActivate.SetActive(active);
        else
            gameObject.SetActive(active);
    }

    public void SetActiveTrue()
    {
        SetActive(true);
    }

    public void SetActiveFalse()
    {
        SetActive(false);
    }
}
