using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    public float strength = 5f;
    public int shakes = 10;
    public float duration = 1f;
    public float maxAmplitude = 10;

    private Vector3 target;
    private Vector3 defaultPos;
    private float interval;
    private bool shaking = false;

    private Vector3 prevDirection;

    private void Start()
    {
        defaultPos = transform.position;
        target = transform.position;
        interval = duration / shakes;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime/duration);

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartShake();
        }
    }

    public void StartShake()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        shaking = true;
        float elapsedTime = 0;

        while (elapsedTime <= duration)
        {
            Vector3 rndDir = RandomDirection();

            if (Vector3.Distance(target + rndDir * strength, defaultPos) > maxAmplitude)
                target += (rndDir * strength - rndDir * (Vector3.Distance(target + rndDir * strength, defaultPos) - maxAmplitude)); //cant go further out than maxAmplitude
            else
                target += RandomDirection() * strength;

            yield return new WaitForSeconds(interval);

            elapsedTime += interval;
        }

        target = defaultPos;
        shaking = false;
    }

    Vector3 RandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
    }

    public bool GetShaking()
    {
        return shaking;
    }
}
