using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class ShopKeeper : MonoBehaviour, IDamagable, IInteractable
{
    [System.Serializable]
    public struct HitThreshold
    {
        public int hitTriggerAmmount;
        [TextArea(2,5)] public string warningText;
        public UnityEvent consequence;
    }

    public bool Damagable { get => damagable; }
    public bool Interactable { get => interactable; }
    public string InteractText { get => interactText; }
    public Transform Transform { get => transformOverride ? transformOverride : transform; }
    public bool Interacting { get => interacting; }

    [SerializeField] private HitThreshold[] thresholds;
    [SerializeField] private bool damagable = true;
    [SerializeField] private bool interactable = true;
    [SerializeField] [TextArea(2, 5)] private string interactText; 
    [SerializeField] private Transform transformOverride;
    [SerializeField] private float invulnTimeframe = 0.5f;
    //todo maybe instead of references SO Events later on, but fine like this for nowe since it's all contained on the same GameObject
    [SerializeField] private GameObject openedShop;
    [SerializeField] private GameObject closedShop;

    private bool interacting = false;
    private int currentHits = 0;
    private int thresholdIdx = 0;
    private IEnumerator invulnerableCoroutine;

    private void Start()
    {
        invulnerableCoroutine = GoInvulerable(invulnTimeframe);
    }

    public void TakeDamage(float damage)
    {
        if (thresholdIdx >= thresholds.Length) return;

        currentHits++;
        if(thresholds[thresholdIdx].hitTriggerAmmount <= currentHits)
        {
            thresholds[thresholdIdx].consequence.Invoke();
            thresholdIdx++;
            StartCoroutine(invulnerableCoroutine);
        }
    }

    public void ShutShop()
    {
        CloseShop();
        interactable = false;
        interacting = false;
    }

    public void Transformation()
    {
        Debug.Log("I will fight you now");
    }

    public void DisplayTresholdWarning()
    {
        Debug.Log(thresholds[thresholdIdx].warningText);
    }

    public void OpenShop()
    {
        openedShop.SetActive(true);
        closedShop.SetActive(false);
    }

    public void CloseShop()
    {
        openedShop.SetActive(false);
        closedShop.SetActive(true);
    }

    public void Interact()
    {
        OpenShop();
        interactable = false;
        interacting = true;
    }

    public void EndInteract()
    {
        CloseShop();
        interactable = true;
        interacting = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (interacting && other.GetComponent<PlayerController>())
        {
            EndInteract();
        }
    }

    IEnumerator GoInvulerable(float timeframe)
    {
        damagable = false;
        yield return new WaitForSeconds(timeframe);
        damagable = true;
    }

    private void OnDisable()
    {
        StopCoroutine(invulnerableCoroutine);
        damagable = true;
    }
}
