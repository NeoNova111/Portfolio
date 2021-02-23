using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource source;

    public void PlayClip(SoundClip clip)
    {
        clip.PlayClip(source);
    }
}
