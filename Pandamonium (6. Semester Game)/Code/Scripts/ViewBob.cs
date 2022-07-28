using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBob : MonoBehaviour
{
    [SerializeField] private bool useBob = true;

    [SerializeField, Range(0, 0.02f)] private float amplitude = 0.015f;
    [SerializeField, Range(0, 30f)] private float frequency = 10f;
    [SerializeField, Range(1, 5f)] private float recenterSpeed = 3f;

    [SerializeField] private Transform cameraHolder = null;
    [SerializeField] private Transform cameraLookAt = null;

    private Vector3 startPos;
    private PlayerController player;
    private float previousDirectionSign = -1f;

    private void Start()
    {
        player = PlayerController.Instance;
        startPos = cameraHolder.localPosition;
    }

    private void Update()
    {
        if (!useBob) return;

        CheckMotion();
        //Camera.main.transform.LookAt(cameraLookAt, Vector3.up);
    }

    private Vector3 FootstepMotion()
    {
        Vector3 pos = Vector3.zero;
        float currentDirectionSign;
        float sin = Mathf.Sin(Time.time * frequency) * amplitude;
        float cos = Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;

        pos.y += sin;
        pos.x += cos;
        currentDirectionSign = Mathf.Sign(sin);

        if (previousDirectionSign == -1f && currentDirectionSign == 1f)
        {
            Debug.Log("step");
            AkSoundEngine.PostEvent("Player_Foot", gameObject); // play stepsound if stepping
        }

        previousDirectionSign = currentDirectionSign;
        return pos;
    }

    private void CheckMotion()
    {
        if (!player.IsGrounded() || !player.IsActivelyMoving())
        {
            ReturnToRestuingPosition();
            return;
        }

        PlayMotion(FootstepMotion());
    }

    private void PlayMotion(Vector3 motion)
    {
        cameraHolder.localPosition += motion;
    }

    private void ReturnToRestuingPosition()
    {
        if (cameraHolder.localPosition == startPos) return;

        cameraHolder.localPosition = Vector3.MoveTowards(cameraHolder.localPosition, startPos, recenterSpeed * Time.deltaTime);
    }
}
