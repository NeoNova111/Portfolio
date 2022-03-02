using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAfterDelay : MonoBehaviour
{
    [SerializeField] string LevelName;
    private float currentWaitTime = 0f;
    [SerializeField] private float delay;

    // Update is called once per frame
    void Update()
    {
        currentWaitTime += Time.deltaTime;
        if(currentWaitTime >= delay)
        {
            SceneManager.LoadScene(LevelName);
        }
    }
}
