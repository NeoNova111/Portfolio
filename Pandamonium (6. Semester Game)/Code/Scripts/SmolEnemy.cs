using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmolEnemy : MonoBehaviour, IPetable
{
    public ParticleSystem hearts;

    [SerializeField] private Transform customTransform;
    [SerializeField] private bool petable;
    public bool Petable { get => petable; }

    private bool petting;
    public bool Petting { get => petting; set => petting = value; }

    public Transform TargetTransform { get => customTransform ? customTransform : transform; }

    public GameEvent startPet;
    public GameEvent stopPet;

    public AK.Wwise.Event voice;
    public AK.Wwise.Event purr;
    private uint purrID;

    public Vector2 rndVoiceTimer = new Vector2(15f, 20f);
    private IEnumerator voiceLoop;

    private Animator anim;

    public bool IsBoss = false;
    public GameEvent Gameend;

    private bool beingPet = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        voiceLoop = VoiceLoop();
        StartCoroutine(voiceLoop);
    }

    private void Update()
    {
        if(petting && !beingPet)
        {
            beingPet = true;
            hearts.Play();
            startPet.Raise();
            anim.SetBool("petting", true);
            purrID = purr.Post(gameObject);

            if (IsBoss) Gameend.Raise();
        }
        else if (!petting && beingPet)
        {
            beingPet = false;
            hearts.Stop();
            stopPet.Raise();
            anim.SetBool("petting", false);
            AkSoundEngine.StopPlayingID(purrID);
        }
    }

    private void LateUpdate()
    {
        petting = false;
    }

    public void Pet()
    {
        petting = true;
    }

    private IEnumerator VoiceLoop()
    {
        voice.Post(gameObject);
        while (true)
        {
            float rnd = Random.Range(rndVoiceTimer.x, rndVoiceTimer.y);
            yield return new WaitForSeconds(rnd);
            voice.Post(gameObject);
        }
    }
}
