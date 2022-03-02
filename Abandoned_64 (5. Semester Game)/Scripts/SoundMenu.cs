using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundMenu : MonoBehaviour
{
    public SoundSettings customPlayerSettings;

    public Slider master;
    public InputField masterField;
    public Slider music;
    public InputField musicField;
    public Slider sfx;
    public InputField sfxField;

    public AudioMixer mixer;

    private void OnEnable()
    {
        ResetSettings();
    }

    public void SaveSettings()
    {
        customPlayerSettings.masterVolume = master.value;
        customPlayerSettings.musicVolume = music.value;
        customPlayerSettings.sfxVolume = sfx.value;

        SaveManager.Instance.SaveSettingsData();
    }

    private void OnDisable()
    {
        ResetSettings();
    }

    public void ResetSettings()
    {
        if (!SaveManager.Instance || !SaveManager.Instance.SettingsSaveFileExists())
        {
            customPlayerSettings.masterVolume = 1;
            customPlayerSettings.musicVolume = 1;
            customPlayerSettings.sfxVolume = 1;
        }

        master.value = customPlayerSettings.masterVolume;
        masterField.text = (customPlayerSettings.masterVolume * 100).ToString("F1");
        SetMasterVolume(customPlayerSettings.masterVolume);

        music.value = customPlayerSettings.musicVolume;
        musicField.text = (customPlayerSettings.musicVolume * 100).ToString("F1");
        SetMusicVolume(customPlayerSettings.musicVolume);

        sfx.value = customPlayerSettings.sfxVolume;
        sfxField.text = (customPlayerSettings.sfxVolume * 100).ToString("F1");
        SetSFXVolume(customPlayerSettings.sfxVolume);
    }

    public void SetMasterVolume(float v)
    {
        if (!gameObject.activeInHierarchy) //returns look weird but are needed, because the dynamic sliders call this on startup even if the object is disabled
            return;

        mixer.SetFloat("masterVolume", Mathf.Log10(v) * 40);
        masterField.text = (v * 100).ToString("F1");
    }

    public void SetSFXVolume(float v)
    {
        if (!gameObject.activeInHierarchy)
            return;

        mixer.SetFloat("sfxVolume", Mathf.Log10(v) * 40);
        sfxField.text = (v * 100).ToString("F1");
    }

    public void SetMusicVolume(float v)
    {
        if (!gameObject.activeInHierarchy)
            return;

        mixer.SetFloat("musicVolume", Mathf.Log10(v) * 40);
        musicField.text = (v * 100).ToString("F1");
    }
}
