using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameEvent raiseEvent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //SceneManager.LoadSceneAsync(levelName);

            //int sceneCount = SceneManager.sceneCount;
            //for (int i = sceneCount - 1; i >= 0; i--)
            //{
            //    if (SceneManager.GetSceneAt(i).name != levelName)
            //        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
            //}

            playerStats.respawning = false;
            if(raiseEvent != null) { raiseEvent.Raise(); }
            SceneManager.LoadScene(levelName);
        }
    }
}
