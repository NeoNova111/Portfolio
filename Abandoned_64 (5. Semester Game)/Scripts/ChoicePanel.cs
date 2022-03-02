using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour
{
    [SerializeField] private Selectable first;

    private void OnEnable()
    {
        first.Select();
    }
}
