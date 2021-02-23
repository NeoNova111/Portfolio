using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AudioClip")]
public class SoundClip : ScriptableObject
{
    public AudioClip[] clips;
    public RangedFloat clipVolume;
    public RangedFloat clipPitch;
    private AudioManager audioManager;

    public void PlayClip(AudioSource source)
    {
        if (clips.Length == 0) return;
        source.clip = clips[Random.Range(0, clips.Length)];
        source.volume = Random.Range(clipVolume.minValue, clipVolume.maxValue) * AudioManager.instance.masterVolume;
        source.pitch = Random.Range(clipPitch.minValue, clipPitch.maxValue);
        source.Play();
    }
}
