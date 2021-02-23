using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Compass : MonoBehaviour
{
    [SerializeField]
    private Image compassArrow;

    GameObject spawnedEmpty;
    Transform compassTarget;
    //maybe add submarine position to submarine stats
    public SubStats submarineStats;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("loaded");
        spawnedEmpty = new GameObject("used for compass looking at waypoint");

        if (!GameObject.FindGameObjectWithTag("Waypoint"))
        {
            Debug.LogWarning("No object with the Waypoint tag in this scene, compass will just not work");
            return;
        }

        compassTarget = GameObject.FindGameObjectWithTag("Waypoint").transform;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!compassTarget)
        {
            Debug.LogWarning("No object with the Waypoint tag in this scene, compass will just not work");
            return;
        }

        spawnedEmpty.transform.position = submarineStats.submarinePosition;
        spawnedEmpty.transform.LookAt(compassTarget);

        compassArrow.rectTransform.eulerAngles = new Vector3(0, 0, -spawnedEmpty.transform.eulerAngles.y);
    }

    public void SetTarget(Transform newTarget)
    {
        compassTarget = newTarget;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
