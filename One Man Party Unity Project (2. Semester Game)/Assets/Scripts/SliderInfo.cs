using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderInfo : MonoBehaviour
{
    public Image turnIcon;

    public void SetImage(Sprite image)
    {
        turnIcon.sprite = image;
    }
}
