using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableBackgroundglitch : MonoBehaviour
{
    [SerializeField] private GameObject GlitchBackground;
    [SerializeField] private bool Activation = false;
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
        if(!Activation)
        GlitchBackground.SetActive(false);
        if(Activation)
        GlitchBackground.SetActive(true);
    }
}
