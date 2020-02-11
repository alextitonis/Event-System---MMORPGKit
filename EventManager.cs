using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : LiteNetLibBehaviour
{
    public static EventManager getInstance;
    void Awake() { getInstance = this; }


    [SerializeField] float delayForNewEvent = 50f;
    [SerializeField] GameObject[] events;

    public IEvent currentEvent { get; private set; }

    void Start()
    {
        Invoke("StartNewEvent", delayForNewEvent);
    }

    public void EventFinished(IEvent e)
    {
        if (e == null)
            return;

        if (e == currentEvent)
            currentEvent = null;

        Invoke("StartNewEvent", delayForNewEvent);
    }


    void StartNewEvent()
    {
        if (events.Length == 0)
            return;

        GameObject e = null;

        if (events.Length == 1)
            e = events[0];
        else
            e = events[Random.Range(0, events.Length - 1)];

        currentEvent = e.GetComponent<IEvent>();
    }
}