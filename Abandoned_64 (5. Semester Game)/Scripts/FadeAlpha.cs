using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAlpha : MonoBehaviour
{
    [SerializeField] private float fadeStartDistance;
    [SerializeField] private float fadeFinishDistance;
    [SerializeField] private float fadePerSecond = 0.7f;
    private PlayerStateMachine player;
    private Material material;
    private float currentAlpha = 1;
    private Camera camera;

    private void Start()
    {
        player = PlayerStateMachine.Instance;
        camera = Camera.main;
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        Vector3 planarPlayerPos = Vector3.ProjectOnPlane(player.transform.position, Vector3.up);
        Vector3 planarPosition = Vector3.ProjectOnPlane(transform.position, Vector3.up);
        float playerDistance = Vector3.Distance(planarPlayerPos, planarPosition);
        Vector3 planarCameraPos = Vector3.ProjectOnPlane(camera.transform.position, Vector3.up);
        float cameraDistance = Vector3.Distance(planarCameraPos, planarPosition);
        //if(playerDistance <= fadeFinishDistance)
        //{
        //    Material material = GetComponent<Renderer>().material;
        //    Color color = material.color;
        //    material.color = new Color(color.r, color.g, color.b, 0);
        //}
        //else if(playerDistance <= fadeStartDistance)
        //{
        //    Material material = GetComponent<Renderer>().material;
        //    Color color = material.color;
        //    float alpha = (playerDistance - fadeFinishDistance) / (fadeStartDistance - fadeFinishDistance);
        //    Debug.Log(alpha);

        //    material.color = new Color(color.r, color.g, color.b, alpha);
        //}

        if (playerDistance <= fadeStartDistance|| cameraDistance <= fadeStartDistance)
        {
            currentAlpha = Mathf.Clamp01(currentAlpha - (fadePerSecond * Time.deltaTime));
        }
        else if(cameraDistance >= Vector3.Distance(planarCameraPos, planarPlayerPos))
        {
            currentAlpha = Mathf.Clamp01(currentAlpha + (fadePerSecond * Time.deltaTime));
        }
        material.SetFloat("Vector1_ScriptFade", currentAlpha);
    }
}
