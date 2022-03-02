using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugModeManager : MonoBehaviour
{
    #region Singeton

    private static DebugModeManager instance;

    public static DebugModeManager Instance { get => instance; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    [SerializeField] private GameEvent enterDebugMode;
    [SerializeField] private GameEvent exitDebugMode;

    private bool debugModeActive = false;
    public bool DebugModeActive { get => debugModeActive;  }

    private void Update()
    {
        
    }

    public void ToggleDebugMode()
    {
        if (debugModeActive)
        {
            debugModeActive = false;
            exitDebugMode.Raise();
        }
        else
        {
            debugModeActive = true;
            enterDebugMode.Raise();
        }
    }
}
