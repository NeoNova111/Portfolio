using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTransitionTrigger : MonoBehaviour
{
    public AudioClip musicToChangeTo;
    [Range(0, 30f)]
    public float transitionTime = 5f;
    AudioManager audiomanager;

    private void Start()
    {
        audiomanager = AudioManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            audiomanager.StartTransitionMusic(transitionTime, musicToChangeTo);
        }
    }
}
