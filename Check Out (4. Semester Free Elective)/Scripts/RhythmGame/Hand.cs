using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] Transform rest;
    [SerializeField] Transform scanned;
    [SerializeField] Transform holdPos;

    public float actionSpeed = 30;
    public float defaultSpeed = 1;

    Queue<IEnumerator> routineQueue;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PickUp(Transform target)
    {
        StopAllCoroutines();
        Coroutine scan = StartCoroutine(ScanProduce(target));
    }

    public void Discard(Transform target)
    {
        StopAllCoroutines();
        Coroutine pickup = StartCoroutine(DiscardProduce(target));
    }

    public void Hold()
    {
        //StopAllCoroutines();
        //Coroutine hold = StartCoroutine(HoldProduce());
    }

    IEnumerator DiscardProduce(Transform target)
    {
        Vector3 targetPos = target.position;
        while (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, actionSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        while (Vector3.Distance(rest.position, transform.position) >= 2)
        {
            transform.position = Vector3.MoveTowards(transform.position, rest.position, actionSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        while (transform.position != rest.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, rest.position, defaultSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        NextRoutine();
    }

    IEnumerator ScanProduce(Transform target)
    {
        Vector3 targetPos = target.position;
        while (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, actionSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        //target.transform.SetParent(transform);

        while (transform.position != scanned.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, scanned.position, actionSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        while (transform.position != rest.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, rest.position, defaultSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        NextRoutine();
    }

    //IEnumerator HoldProduce()
    //{
    //    while (transform.position != holdPos.position)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, holdPos.position, actionSpeed * Time.deltaTime);
    //        yield return new WaitForEndOfFrame();
    //    }

    //    while (transform.position != rest.position)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, rest.position, defaultSpeed * Time.deltaTime);
    //        yield return new WaitForEndOfFrame();
    //    }

    //    NextRoutine();
    //}

    void NextRoutine()
    {

    }
}
