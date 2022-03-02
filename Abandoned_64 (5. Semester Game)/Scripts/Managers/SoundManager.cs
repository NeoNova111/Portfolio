using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private List<AudioSource> audioSources;
    public List<AudioSource> AudioSources { get => audioSources; }

    public void Awake()
    {
        audioSources = new List<AudioSource>();
        AudioSource[] sources = GetComponents<AudioSource>();
        foreach (AudioSource source in sources)
        {
            audioSources.Add(source);
        }
    }

    public void PlayClip(SoundClip clip)
    {
        clip.PlayClip(audioSources[GetFreeAudioSourceIdx()]);
    }

    public void PlayClip(AudioClip clip)
    {
        AudioSource freeSource = audioSources[GetFreeAudioSourceIdx()];
        freeSource.clip = clip;
        freeSource.Play();
    }

    public void StopAllClipsPlaying()
    {
        foreach (AudioSource source in audioSources)
        {
            source.Stop();
        }
    }

    public void PauseAll()
    {
        foreach (AudioSource source in audioSources)
        {
            source.Pause();
        }
    }

    public void UnpauseAll()
    {
        foreach (AudioSource source in audioSources)
        {
            source.UnPause();
        }
    }

    public bool IsAnyPlaying()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            if (audioSources[i].isPlaying)
                return true;
        }
        return false;
    }

    int GetFreeAudioSourceIdx()
    {
        for(int i = 0; i < audioSources.Count; i++)
        {
            if (!audioSources[i].isPlaying)
            {
                return i;
            }
        }
        return 0;
    }

    public float LengthOfClip(int idx)
    {
        if (!audioSources[idx].clip || idx > audioSources.Count - 1)
            return 0;

        return audioSources[idx].clip.length;
    }

    //public void UpdateVolume(float volume)
    //{
    //    foreach (AudioSource source in audioSources)
    //    {
    //        source.volume = volume;
    //    }
    //}
}
