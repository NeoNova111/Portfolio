using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType { FadeInOutTransition, CrossfadeTransition, FadeIn, FadeOut, CutIn, CutOut }
public enum TriggerType { Trigger, OnLSceneLoad }

public class MusicTransitionTrigger : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private TriggerType type = TriggerType.Trigger;
    public int triggerAmount = 1;
    [Header("Music")]
    [SerializeField] private TransitionType musicTransitionType;
    [SerializeField] private float musicFadeTime = 1f;
    [SerializeField] private bool fadeMusic = true;
    [SerializeField] private AudioClip newMusic;
    [SerializeField] private float musicVolume = 1f;
    [Header("Ambience")]
    [SerializeField] private TransitionType ambienceTransitionType;
    [SerializeField] private float ambienceFadeTime = 1f;
    [SerializeField] private bool fadeAmbience = true;
    [SerializeField] private AudioClip newAmbience;
    [SerializeField] private float ambienceVolume = 1f;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && type == TriggerType.Trigger)
        {
            Transition();
        }
    }

    private void Transition()
    {
        if (fadeMusic)
        {
            switch (musicTransitionType)
            {
                case TransitionType.CrossfadeTransition:
                    if(newMusic) audioManager.CrossfadeToNewMusic(musicFadeTime, newMusic, targetVolume: musicVolume);
                    break;
                case TransitionType.FadeInOutTransition:
                    if (newMusic) audioManager.StartTransitionMusic(musicFadeTime, newMusic, targetVolume: musicVolume);
                    break;
                case TransitionType.FadeIn:
                    if (newMusic) audioManager.FadeInMusic(musicFadeTime, newMusic, targetVolume: musicVolume);
                    break;
                case TransitionType.FadeOut:
                    audioManager.FadeOutMusic(musicFadeTime);
                    break;
                case TransitionType.CutIn:
                    if (newMusic) audioManager.SetMusic(newMusic);
                    break;
                case TransitionType.CutOut:
                    audioManager.StopMusic();
                    break;
                default:
                    break;
            }
        }

        if (fadeAmbience)
        {
            switch (ambienceTransitionType)
            {
                case TransitionType.CrossfadeTransition:
                    if (newAmbience) audioManager.CrossfadeToNewAmbience(ambienceFadeTime, newAmbience, targetVolume: ambienceVolume);
                    break;
                case TransitionType.FadeInOutTransition:
                    if (newAmbience) audioManager.StartTransitionAmbience(ambienceFadeTime, newAmbience, targetVolume: ambienceVolume);
                    break;
                case TransitionType.FadeIn:
                    if (newAmbience) audioManager.FadeInAmbience(ambienceFadeTime, newAmbience, targetVolume: ambienceVolume);
                    break;
                case TransitionType.FadeOut:
                    audioManager.FadeOutAmbience(ambienceFadeTime);
                    break;
                case TransitionType.CutIn:
                    if (newAmbience) audioManager.SetAmbience(newAmbience);
                    break;
                case TransitionType.CutOut:
                    audioManager.StopAmbience();
                    break;
                default:
                    break;
            }
        }

        triggerAmount--;

        if (triggerAmount == 0)
        {
            gameObject.SetActive(false);
        }
    }

    void OnLevelLoad(Scene scen, LoadSceneMode mode)
    {
        if(type == TriggerType.OnLSceneLoad)
            Transition();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoad;      
    }
}
