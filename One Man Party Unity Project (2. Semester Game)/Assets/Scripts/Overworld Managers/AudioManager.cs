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

    public AudioSource audioSource;
    public AudioSource[] battleStems;
    public AudioClip battleTheme;
    public AudioClip villageTheme;
    [Range(0f,1f)]
    public float masterVolume = 1f;

    public bool indoors;
    public float indoorsVolume = 1f;
    public float outdoorsVolume = 1f;

    [SerializeField]
    private AudioMixerSnapshot supportSnapshot;
    [SerializeField]
    private AudioMixerSnapshot attackSnapshot;
    [SerializeField]
    private AudioMixerSnapshot defenseSnapshot;
    [SerializeField]
    private AudioMixerSnapshot menuSnapshot;
    [SerializeField]
    private AudioMixerSnapshot defaultSnapshot;

    public float snapshotTransitionTime = 0.1f; 

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetIndoors(indoors);
    }

    private void Update()
    {
        foreach(AudioSource stem in battleStems)
        {
            stem.volume = masterVolume;
        }
    }

    public void SetMusic(AudioClip newClip)
    {
        audioSource.clip = newClip;
        StartMusic();
    }

    public void StartMusic()
    {
        audioSource.Play();
    }

    public void SyncMusic()
    {
        audioSource.Play();
        foreach(AudioSource stem in battleStems)
        {
            stem.Play();
        }
    }


    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        audioSource.volume = volume * masterVolume;
    }

    public void UpdateVolume()
    {
        SetIndoors(indoors);
    }

    public void SetIndoors(bool newIndoors)
    {
        indoors = newIndoors;
        if (indoors)
        {
            SetMusicVolume(indoorsVolume);
        }
        else
        {
            SetMusicVolume(outdoorsVolume);
        }
    }

    #region Snapshot Triggers

    public void TriggerSupportSnapshot()
    {
        supportSnapshot.TransitionTo(snapshotTransitionTime);
    }

    public void TriggerAttackSnapshot()
    {
        attackSnapshot.TransitionTo(snapshotTransitionTime);
    }

    public void TriggerDefenseSnapshot()
    {
        defenseSnapshot.TransitionTo(snapshotTransitionTime);
    }

    public void TriggerMenuSnapshot()
    {
        menuSnapshot.TransitionTo(0f);
    }

    public void TriggerDefaultSnapshot()
    {
        defaultSnapshot.TransitionTo(0f);
    }

    #endregion

    public void CheckJobMusic()
    {
        Job job = BattleSystem.instance.playerUnit.job;

        switch(job.type)
        {
            case JobType.SUPPORT:
                //Debug.Log("support");
                TriggerSupportSnapshot();
                break;
            case JobType.DEFENSE:
                //Debug.Log("Defense");
                TriggerDefenseSnapshot();
                break;
            case JobType.OFFENSE:
                //Debug.Log("offense");
                TriggerAttackSnapshot();
                break;
            default:
                Debug.LogError("No fitting job type", this);
                break;
        }
    }
}
