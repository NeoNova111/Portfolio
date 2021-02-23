using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SanitySliderControl : MonoBehaviour
{
    public Crewmember crewmate;
    public Slider slider;

    // Update is called once per frame
    void Update()
    {
        slider.value = crewmate.sanity;
    }
}
