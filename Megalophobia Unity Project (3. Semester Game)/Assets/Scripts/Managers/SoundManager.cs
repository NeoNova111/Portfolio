using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource[] audioSources;

    private void Awake()
    {
        audioSources = gameObject.GetComponents<AudioSource>();
    }

    public void PlayClip(SoundClip clip)
    {
        clip.PlayClip(audioSources[GetFreeAudioSourceIdx()]);
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
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i].isPlaying)
                return true;
        }
        return false;
    }

    int GetFreeAudioSourceIdx()
    {
        for(int i = 0; i < audioSources.Length; i++)
        {
            if (!audioSources[i].isPlaying)
                return i;
        }
        return 0;
    }

    public float LengthOfClip(int idx)
    {
        if (!audioSources[idx].clip || idx > audioSources.Length - 1)
            return 0;

        return audioSources[idx].clip.length;
    }

    public AudioSource[] GetSources()
    {
        return audioSources;
    }

    //public void UpdateVolume(float volume)
    //{
    //    foreach (AudioSource source in audioSources)
    //    {
    //        source.volume = volume;
    //    }
    //}
}
