using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMovementSFX : MonoBehaviour
{
    public AudioSource turbine;
    [Range(0, 1f)]
    public float turbineVolume = 1f;
    public float maxTurbinePitch = 2f;

    public AudioSource bubbles;
    [Range(0, 1f)]
    public float bubblesVolume = 1f;

    public AudioSource motor;
    [Range(0, 0.02f)]
    public float idleMotorVolume = 0.2f;
    [Range(0, 1f)]
    public float motorVolume = 1f;
    public float maxMotorPitch = 2f;

    public float startupTime = 1f;
    float time;

    public float muteStartTime = 5f;
    public float finishMuteTiume = 10f;
    [Range (0, 1f)]
    public float muteStrength;

    float masterVolume;
    float sfxVolume;

    AudioManager audioManager;

    private void Awake()
    {
        time = startupTime;
    }

    private void Start()
    {
        audioManager = AudioManager.instance;
        masterVolume = audioManager.currentSoundSettings.masterVolume;
        sfxVolume = audioManager.currentSoundSettings.sfxVolume;
    }

    private void Update()
    {
        //side ignores turning for now (polishing)
        if (submarineMovement.instance.GetCurrentRoom().perspective == CameraPerspective.SIDE)
        {
            if (Input.GetAxis("Vertical") != 0 && time < startupTime)
            {
                time += Time.deltaTime;
            }
            else if (Input.GetAxis("Horizontal") != 0 && time < startupTime && !submarineMovement.instance.IsRotating())
            {
                time += Time.deltaTime;
            }
            else if (time > 0)
            {
                time -= Time.deltaTime;
            }
        }
        else
        {
            if (Input.GetAxis("Vertical") > 0 && time < startupTime)
            {
                time += Time.deltaTime;
            }
            else if (Input.GetAxis("Vertical") < 0 && time < startupTime)
            {
                time += Time.deltaTime / 3;
            }
            else if (Input.GetAxisRaw("Vertical") == 0 && time > 0)
            {
                time -= Time.deltaTime;
            }
        }

        AdjustMovementVolume();
        AdjustMovementPitch();
    }

    void AdjustMovementVolume()
    {
        float t = time / startupTime;

        bubbles.volume = Mathf.Lerp(0, bubblesVolume, t);
        turbine.volume = Mathf.Lerp(0, turbineVolume, t);
        motor.volume = Mathf.Lerp(idleMotorVolume, motorVolume, t);
    }

    void AdjustMovementPitch()
    {
        float t = time / startupTime;

        turbine.pitch = Mathf.Lerp(1, maxTurbinePitch, t);
        motor.pitch = Mathf.Lerp(1, maxMotorPitch, t);
    }
}
