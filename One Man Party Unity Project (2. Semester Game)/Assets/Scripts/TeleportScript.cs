using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportScript : MonoBehaviour
{
    public bool changeScene = false;
    public string sceneName;

    public Vector3 destination;
    public bool isDestinationIndoors = false;

    public bool destinationHasNewMusic = false;
    public AudioClip newMusic;

    public Animator transition;
    public float transitionTime = 1f;

    public GameObject transitionObj;

    void TpToDestination(GameObject objectToTp)
    {
        StaticInfo.playerPos = destination;
        AudioManager.instance.SetIndoors(isDestinationIndoors);

        if (changeScene)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().DeactivatePlayer();
            StaticInfo.gotEnemiesInScenes = false;
            StaticInfo.loadPlayerInfo = true;
            LoadLevelStart(sceneName);
        }
        else
        {
            objectToTp.transform.position = destination;
            if (destinationHasNewMusic && newMusic != null)
            {
                AudioManager.instance.SetMusic(newMusic);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            transitionObj.SetActive(true);
            TpToDestination(collision.gameObject);
        }
    }

    public void LoadLevelStart(string scene)
    {
        StartCoroutine(Loadlevel(scene));
        StaticInfo.enemyDefeatStatus = new List<bool>();
        StaticInfo.gotEnemiesInScenes = false;
    }

    IEnumerator Loadlevel(string scene)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        if (destinationHasNewMusic && newMusic != null)
        {
            AudioManager.instance.SetMusic(newMusic);
        }
        SceneManager.LoadScene(scene);
    }
}
