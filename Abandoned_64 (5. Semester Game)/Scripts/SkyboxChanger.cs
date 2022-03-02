using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    [SerializeField] Material Sky;
    [SerializeField] float FogDensity;
    [SerializeField] Color FogColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    private void OnTriggerEnter(Collider other)
    {
        RenderSettings.skybox = Sky;
        RenderSettings.fogDensity = FogDensity;
        RenderSettings.fogColor = FogColor;
    }
   
}
