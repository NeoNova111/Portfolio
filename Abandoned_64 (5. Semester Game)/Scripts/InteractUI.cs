using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [SerializeField] GameObject Jump;
    [SerializeField] GameObject Interact;
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
        Jump.SetActive(false);
        Interact.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        Jump.SetActive(true);
        Interact.SetActive(false);
    }
}
