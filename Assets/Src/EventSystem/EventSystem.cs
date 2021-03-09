using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public static EventSystem self;
    public enum EventType {
        ui
    };

    protected Action<GameObject> uiEvents;
    protected Dictionary<EventType, Action<GameObject>> events;

    void Awake()
    {
        self = this;
        events = new Dictionary<EventType, Action<GameObject>>();

        events.Add(EventType.ui, uiEvents);
    }

    public bool SendEvent(EventType type, GameObject actor) {
        events[type]?.Invoke(actor);

        return true;
    }

    public bool Subscribe(EventType type, Action<GameObject> callback) {
        events[type] += callback;

        return true;
    }
}
