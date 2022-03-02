using System;
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

    [SerializeField] private float defaultFadeTime = 1f;
    private const int fadeArraySize = 2;
    private int currentMusicSourceIndex = 0;
    private int currentAmbienceSourceIndex = 0;

    [SerializeField] private AudioClip currentMusic;
    [SerializeField] private AudioSource[] musicFadeSources;

    [SerializeField] private AudioClip currentAmbience;
    [SerializeField] private AudioSource[] ambienceFadeSources;

    private IEnumerator currentlyRunningAmbienceFade;
    private IEnumerator currentlyRunningMusicFade;

    [Header("mixing Stuff")]
    public SoundSettings currentSoundSettings;

    public SoundManager UISFX;
    public SoundManager ingameSFX;

    public AudioMixerSnapshot unpaused;
    public AudioMixerSnapshot paused;

    public AudioMixer mixer;

    private void Start()
    {
        SaveManager managerInstance = SaveManager.Instance;

        managerInstance.LoadSettingsData();

        if (managerInstance.SettingsSaveFileExists())
        {
            SetMastervolume(managerInstance.SettingsLoadData.masterVolume);
            SetMusicVolume(managerInstance.SettingsLoadData.musicVolume);
            SetSFXVolume(managerInstance.SettingsLoadData.sfxVolume);
        }

        SetMusic(currentMusic);
        SetAmbience(currentAmbience);
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
        musicFadeSources[currentMusicSourceIndex].clip = newClip;
        currentMusic = newClip;
        PlayMusic();
    }

    public void PlayMusic()
    {
        musicFadeSources[currentMusicSourceIndex].Play();
    }

    public void PauseMusic()
    {
        musicFadeSources[currentMusicSourceIndex].Pause();
        musicFadeSources[currentMusicSourceIndex].volume = 0;
    }

    public void StopMusic()
    {
        musicFadeSources[currentMusicSourceIndex].Stop();
        musicFadeSources[currentMusicSourceIndex].volume = 0;
    }

    public void SetAmbience(AudioClip newClip)
    {
        ambienceFadeSources[currentAmbienceSourceIndex].clip = newClip;
        currentAmbience = newClip;
        PlayAmbience();
    }

    public void PlayAmbience()
    {
        ambienceFadeSources[currentAmbienceSourceIndex].Play();
    }

    public void PauseAmbience()
    {
        ambienceFadeSources[currentAmbienceSourceIndex].Pause();
        ambienceFadeSources[currentAmbienceSourceIndex].volume = 0;
    }

    public void StopAmbience()
    {
        ambienceFadeSources[currentAmbienceSourceIndex].Stop();
        ambienceFadeSources[currentAmbienceSourceIndex].volume = 0;
    }

    public void StartTransitionMusic(float time, AudioClip newMusic, float targetVolume = 1f)
    {
        if (currentlyRunningMusicFade != null)
        {
            StopCoroutine(currentlyRunningMusicFade);
        }

        currentlyRunningMusicFade = TransitionMusic(time, newMusic, targetVolume);
        StartCoroutine(currentlyRunningMusicFade);
    }

    public void StartTransitionAmbience(float time, AudioClip newAmbience, float targetVolume = 1f)
    {
        if (currentlyRunningAmbienceFade != null)
        {
            StopCoroutine(currentlyRunningAmbienceFade);
        }

        currentlyRunningAmbienceFade = TransitionAmbience(time, newAmbience, targetVolume);
        StartCoroutine(currentlyRunningAmbienceFade);
    }

    IEnumerator TransitionMusic(float time, AudioClip newMusic, float targetVolume)
    {
        float fadeoutTime = (time / 2) * musicFadeSources[currentMusicSourceIndex].volume;
        float fadeInTime = time / 2;
        float startVolume = musicFadeSources[currentMusicSourceIndex].volume;
        time = 0;

        while (time <= fadeoutTime / 2)
        {
            time += Time.deltaTime;
            musicFadeSources[currentMusicSourceIndex].volume = Mathf.Lerp(startVolume, 0, time / (fadeoutTime / 2));
            yield return null;
        }

        SetMusic(newMusic);
        time = 0;

        while (time <= fadeInTime)
        {
            time += Time.deltaTime;
            musicFadeSources[currentMusicSourceIndex].volume = Mathf.Lerp(0, targetVolume, time / fadeInTime);
            yield return null;
        }
    }

    IEnumerator TransitionAmbience(float time, AudioClip newAmbience, float targetVolume)
    {
        float fadeoutTime = (time / 2) * ambienceFadeSources[currentAmbienceSourceIndex].volume;
        float fadeInTime = time / 2;
        float startVolume = musicFadeSources[currentAmbienceSourceIndex].volume;
        time = 0;

        while (time <= fadeoutTime)
        {
            time += Time.deltaTime;
            ambienceFadeSources[currentAmbienceSourceIndex].volume = Mathf.Lerp(startVolume, 0, time / fadeoutTime);
            yield return null;
        }

        SetAmbience(newAmbience);
        time = 0;

        while (time <= fadeInTime)
        {
            time += Time.deltaTime;
            ambienceFadeSources[currentAmbienceSourceIndex].volume = Mathf.Lerp(0, targetVolume, time / fadeInTime);
            yield return null;
        }
    }

    public void CrossfadeToNewMusic(float fadeTime, AudioClip newMusic, float targetVolume = 1f)
    {
        if (currentlyRunningMusicFade != null)
        {
            StopCoroutine(currentlyRunningMusicFade);
        }

        currentMusicSourceIndex = 1 - currentMusicSourceIndex;
        SetMusic(newMusic);
        currentlyRunningMusicFade = CrossfadeMusic(musicFadeSources, currentMusicSourceIndex, fadeTime, targetVolume);
        StartCoroutine(currentlyRunningMusicFade);;
    }

    public void CrossfadeToNewAmbience(float fadeTime, AudioClip newAmbience, float targetVolume = 1f)
    {
        if (currentlyRunningAmbienceFade != null)
        {
            StopCoroutine(currentlyRunningAmbienceFade);
        }

        currentAmbienceSourceIndex = 1 - currentAmbienceSourceIndex;
        SetAmbience(newAmbience);
        currentlyRunningAmbienceFade = CrossfadeAmbience(ambienceFadeSources, currentAmbienceSourceIndex, fadeTime, targetVolume);
        StartCoroutine(currentlyRunningAmbienceFade);
    }

    IEnumerator CrossfadeMusic(AudioSource[] sources, int currentAudioSourceIndex, float fadeTime, float targetVolume)
    {
        float currentTime = 0;
        float startVolume = musicFadeSources[currentMusicSourceIndex].volume;

        while (currentTime < fadeTime)
        {
            currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, fadeTime);
            sources[currentAudioSourceIndex].volume = Mathf.Lerp(0, targetVolume, currentTime / fadeTime);
            sources[1 - currentAudioSourceIndex].volume = Mathf.Lerp(startVolume, 0, currentTime / fadeTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CrossfadeAmbience(AudioSource[] sources, int currentAudioSourceIndex, float fadeTime, float targetVolume)
    {
        float currentTime = 0;
        float startVolume = musicFadeSources[currentAmbienceSourceIndex].volume;

        while (currentTime < fadeTime)
        {
            currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, fadeTime);
            sources[currentAmbienceSourceIndex].volume = Mathf.Lerp(0, targetVolume, currentTime / fadeTime);
            sources[1 - currentAmbienceSourceIndex].volume = Mathf.Lerp(startVolume, 0, currentTime / fadeTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public void FadeOutMusic(float fadeTime)
    {
        if (currentlyRunningMusicFade != null)
        {
            StopCoroutine(currentlyRunningMusicFade);
        }
        currentlyRunningMusicFade = FadeSource(musicFadeSources[currentMusicSourceIndex], fadeTime, 0f);
        StartCoroutine(currentlyRunningMusicFade);
    }

    public void FadeOutAmbience(float fadeTime)
    {
        if (currentlyRunningAmbienceFade != null)
        {
            StopCoroutine(currentlyRunningAmbienceFade);
        }
        currentlyRunningAmbienceFade = FadeSource(ambienceFadeSources[currentAmbienceSourceIndex], fadeTime, 0f);
        StartCoroutine(currentlyRunningAmbienceFade);
    }

    public void FadeInMusic(float fadeTime, AudioClip newMusic, float targetVolume = 1f)
    {
        if (currentlyRunningMusicFade != null)
        {
            StopCoroutine(currentlyRunningMusicFade);
        }

        SetMusic(newMusic);
        currentlyRunningMusicFade = FadeSource(musicFadeSources[currentMusicSourceIndex], fadeTime, targetVolume);
        StartCoroutine(currentlyRunningMusicFade);
    }

    public void FadeInAmbience(float fadeTime, AudioClip newMusic, float targetVolume = 1f)
    {
        if (currentlyRunningAmbienceFade != null)
        {
            StopCoroutine(currentlyRunningAmbienceFade);
        }

        SetAmbience(newMusic);
        currentlyRunningAmbienceFade = FadeSource(ambienceFadeSources[currentAmbienceSourceIndex], fadeTime, targetVolume);
        StartCoroutine(currentlyRunningAmbienceFade);
    }

    IEnumerator FadeSource(AudioSource source, float fadeTime, float targetVolume)
    {
        float currentTime = 0;
        float startVolume = source.volume;

        while (currentTime < fadeTime)
        {
            currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, fadeTime);
            source.volume = Mathf.Lerp(startVolume, targetVolume , currentTime / fadeTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnValidate()
    {
        if(musicFadeSources.Length != fadeArraySize)
        {
            Debug.LogWarning("Don't change the 'musicFadeSources' field's array size!");
            Array.Resize(ref musicFadeSources, fadeArraySize);
        }

        if (ambienceFadeSources.Length != fadeArraySize)
        {
            Debug.LogWarning("Don't change the 'ambienceFAdeSources' field's array size!");
            Array.Resize(ref ambienceFadeSources, fadeArraySize);
        }
    }
}
