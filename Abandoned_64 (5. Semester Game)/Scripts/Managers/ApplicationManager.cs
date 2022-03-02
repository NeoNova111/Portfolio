using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    [SerializeField] private int width = 450;
    [SerializeField] private int height = 338;
    private Resolution res;

    void Start()
    {
        //res = Screen.currentResolution;
    }

    private void Update()
    {
        Resolution currentRes = Screen.currentResolution;
        if(currentRes.width != width || currentRes.height != height)
        {
            Resize();
        }
    }

    void Resize()
    {
        Screen.SetResolution(width, height, true);
    }
}
