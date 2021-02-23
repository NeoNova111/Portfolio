using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Text volumeValueText;
    public Slider volumeSlider;
    private float volumeValue;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
        UpdateVolume();
    }

    private void Update()
    {
        if(volumeValue != volumeSlider.value)
        {
            UpdateVolume();
        }
    }

    void UpdateVolume()
    {
        volumeValue = volumeSlider.value;
        audioManager.masterVolume = volumeValue;
        audioManager.UpdateVolume();
        volumeValueText.text = ""+ (int)(volumeValue * 100);
    }
}
