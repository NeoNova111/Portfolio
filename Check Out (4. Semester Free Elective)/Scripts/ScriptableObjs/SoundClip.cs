using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundClipType {SFX, VOICE}

[CreateAssetMenu (menuName = "AudioClip")]
public class SoundClip : ScriptableObject
{
    public AudioClip[] clips;
    public SoundClipType clipType = SoundClipType.SFX;
    public RangedFloat clipVolume;
    public RangedFloat clipPitch;
    private AudioManager audioManager;

    public void PlayClip(AudioSource source)
    {
        if (clips.Length == 0) return;
        source.clip = clips[Random.Range(0, clips.Length)];
        source.pitch = Random.Range(clipPitch.minValue, clipPitch.maxValue);
        switch (clipType)
        {
            case SoundClipType.SFX:
                source.volume = Random.Range(clipVolume.minValue, clipVolume.maxValue)/* * AudioManager.instance.currentSoundSettings.masterVolume * AudioManager.instance.currentSoundSettings.sfxVolume*/;
                break;
            case SoundClipType.VOICE:
                source.volume = Random.Range(clipVolume.minValue, clipVolume.maxValue)/* * AudioManager.instance.currentSoundSettings.masterVolume * AudioManager.instance.currentSoundSettings.voiceoverVolume*/;
                break;
        }

        source.Play();
    }
}
