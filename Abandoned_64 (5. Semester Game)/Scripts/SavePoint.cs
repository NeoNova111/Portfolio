using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private bool overwideTransform = false;
    [SerializeField] private Vector3 overridePosition;
    [SerializeField] private Vector3 overrideRotation;

    private SaveManager saveManager;

    private void Start()
    {
        saveManager = SaveManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (overwideTransform)
            {
                saveManager.SavePlayerData(overridePosition, overrideRotation);
            }
            else
            {
                saveManager.SavePlayerData();
            }
        }
    }
}
