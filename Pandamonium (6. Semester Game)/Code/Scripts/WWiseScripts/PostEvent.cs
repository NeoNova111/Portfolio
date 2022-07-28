using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEvent : MonoBehaviour
{
    public bool postOnStart = false;
    public bool postOnEnable = false;
    public bool stopOnDisable = false;

    [SerializeField] private AK.Wwise.Event[] eventsToPost;

    private List<uint> eventIDs;

    private void Awake()
    {
        eventIDs = new List<uint>();
    }

    public void PostEvents()
    {
        foreach(var e in eventsToPost)
        {
            eventIDs.Add(e.Post(gameObject));
        }
    }

    public void StopEvents()
    {
        foreach(var id in eventIDs)
        {
            AkSoundEngine.StopPlayingID(id);
        }
    }

    private void Start()
    {
        if (postOnStart) PostEvents();
    }

    private void OnEnable()
    {
        if (postOnEnable) PostEvents();
    }

    private void OnDisable()
    {
        if (stopOnDisable) StopEvents();
    }
}
