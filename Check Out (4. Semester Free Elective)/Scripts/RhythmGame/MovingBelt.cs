using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBelt : MonoBehaviour
{
    public GameObject movingLinePrefab;
    public float beltDistance = 1.5f;
    List<GameObject> lineObjects;
    public int numberOfBelts = 20;
    float speed = 0;
    // Start is called before the first frame update
    void Start()
    {
        lineObjects = new List<GameObject>();


        lineObjects.Add(Instantiate(movingLinePrefab, transform.position, Quaternion.identity));
        for (int i = 0; i < numberOfBelts - 1; i++)
        {
            lineObjects.Add(Instantiate(movingLinePrefab, lineObjects[lineObjects.Count - 1].transform.position + Vector3.right * beltDistance, Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(speed == 0) //todo: better solution with events?
        {
            SyncSpeed();
        }

        foreach(GameObject obj in lineObjects)
        {
            obj.transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
            if(obj.transform.position.x <= transform.position.x)
            {
                obj.transform.position = Vector3.right * (FindLastX() + beltDistance);
            }
        }
    }

    float FindLastX()
    {
        float x = 0;

        foreach(GameObject obj in lineObjects)
        {
            if(obj.transform.position.x > x)
            {
                x = obj.transform.position.x;
            }
        }

        return x;
    }

    void SyncSpeed()
    {
        RhythmGameManager managerInstance = RhythmGameManager.instance;
        float distance = Mathf.Abs(managerInstance.noteSpawnX - managerInstance.scanLine.transform.position.x);
        if (managerInstance.currentSong)
            speed = distance / managerInstance.currentSong.secondsTillLine;
        else
            speed = 10;
    }
}
