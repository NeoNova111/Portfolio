using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    #region Singelton

    public static AudioManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance != null)
        {
            Debug.LogWarning("More than one inventory instance");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    #endregion

    public AudioSource musicSource;
    public AudioClip musicClip;

    public SoundSettings currentSoundSettings;

    public SoundManager soundManager_ingameSFX;
    public SoundManager soundManager_voiceLines;

    public AudioMixerSnapshot unpaused;
    public AudioMixerSnapshot paused;

    public AudioMixer mixer;

    private void Start()
    {
        SaveManager managerInstance = SaveManager.instance;

        //adaptsavestuff
        //managerInstance.LoadSettingsData();

        //if (managerInstance.SettingsSaveFileExists())
        //{
        //    SetMastervolume(managerInstance.loadSettings.masterVolume);
        //    SetMusicVolume(managerInstance.loadSettings.musicVolume);
        //    SetSFXVolume(managerInstance.loadSettings.sfxVolume);
        //    SetVoiceVolume(managerInstance.loadSettings.voiceVolume);
        //}

        musicSource = GetComponent<AudioSource>();
        musicSource.clip = musicClip;
        StartMusic();
    }

    public void TransitionToPausedSnapshot()
    {
        paused.TransitionTo(0.001f);
    }

    public void TransitionToUnpausedSnapshot()
    {
        unpaused.TransitionTo(0.001f);
    }
    
    public void SetMusic(AudioClip newClip)
    {
        musicSource.clip = newClip;
        StartMusic();
    }

    public void StartTransitionMusic(float time, AudioClip newMusic)
    {
        StartCoroutine(TransitionMusic(time, newMusic));
    }

    IEnumerator TransitionMusic(float time, AudioClip newMusic)
    {
        float startTime = time;
        time = 0;

        while(time <= startTime / 2)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(1, 0, time / (startTime / 2));
            yield return null;
        }

        SetMusic(newMusic);
        time = 0;

        while (time <= startTime / 2)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, 1, time / (startTime / 2));
            yield return null;
        }
    }

    public void StartMusic()
    {
        musicSource.Play();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        currentSoundSettings.musicVolume = volume;
        mixer.SetFloat("musicVolume", Mathf.Log10(volume) * 40);
    }

    public void SetSFXVolume(float volume)
    {
        currentSoundSettings.sfxVolume = volume;
        mixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 40);
    }

    public void SetMastervolume(float volume)
    {
        currentSoundSettings.masterVolume = volume;
        mixer.SetFloat("masterVolume", Mathf.Log10(volume) * 40);
    }

    public void SetVoiceVolume(float volume)
    {
        currentSoundSettings.voiceoverVolume = volume;
        mixer.SetFloat("voiceVolume", Mathf.Log10(volume) * 40);
    }
}
