using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundMenu : MonoBehaviour
{
    public SoundSettings customPlayerSettings;

    public Slider master;
    public Slider music;
    public Slider sfx;
    public Slider voiceover;

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
        customPlayerSettings.voiceoverVolume = voiceover.value;

        SaveManager.instance.SaveSettingsData();
    }

    private void OnDisable()
    {
        ResetSettings();
    }

    public void ResetSettings()
    {
        master.value = customPlayerSettings.masterVolume;
        SetMasterVolume(customPlayerSettings.masterVolume);

        music.value = customPlayerSettings.musicVolume;
        SetMusicVolume(customPlayerSettings.musicVolume);

        sfx.value = customPlayerSettings.sfxVolume;
        SetSFXVolume(customPlayerSettings.sfxVolume);

        voiceover.value = customPlayerSettings.voiceoverVolume;
        SetVoiceVolume(customPlayerSettings.voiceoverVolume);
    }

    public void SetMasterVolume(float v)
    {
        mixer.SetFloat("masterVolume", Mathf.Log10(v) * 40);
    }

    public void SetSFXVolume(float v)
    {
        mixer.SetFloat("sfxVolume", Mathf.Log10(v) * 40);
    }

    public void SetMusicVolume(float v)
    {
        mixer.SetFloat("musicVolume", Mathf.Log10(v) * 40);
    }

    public void SetVoiceVolume(float v)
    {
        mixer.SetFloat("voiceVolume", Mathf.Log10(v) * 40);
    }
}
