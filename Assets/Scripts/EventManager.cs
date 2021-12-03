using System;
using System.Collections.Generic;
using UnityEngine;


public class EventManager : MonoBehaviour
{
    private Dictionary<string, Action<Dictionary<string, object>>> eventDict;
    private static EventManager eventManager;
    
    // get the singleton object
    public static EventManager instance
    {
        get
        {
            if (eventManager == null)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (eventManager == null)
                {
                    Debug.LogError("No EventManager script found on a GameObject!");
                } else
                {
                    eventManager.Init();
                    DontDestroyOnLoad(eventManager);
                }
            }
            return eventManager;
        }
    }

    private void Init() 
    {
        if (eventDict == null)
        {
            eventDict = new Dictionary<string, Action<Dictionary<string, object>>>();
        }        
    }

    public static void StartListening(string eventName, Action<Dictionary<string, object>> listener)
    {
        Action<Dictionary<string, object>> ev;
        if (instance.eventDict.TryGetValue(eventName, out ev))
        {
            ev += listener;
            instance.eventDict[eventName] = ev;
        } else
        {
            ev += listener;            
            instance.eventDict.Add(eventName, ev);
        }
    }

    public static void StopListening(string eventName, Action<Dictionary<string, object>> listener)
    {
        if (eventManager == null)
            return;
        Action<Dictionary<string, object>> ev;
        if (instance.eventDict.TryGetValue(eventName, out ev))
        {
            ev -= listener;
            instance.eventDict[eventName] = ev;
        }
    }

    // How to use
    // no parameter:  EventManager.TriggerEvent("someEvent", null);
    // one parameter: EventManager.TriggerEvent("someEvent", new Dictionary<string, object> { { "enable", true }})
    /* two parameter: EventManager.TriggerEvent("someEvent", new Dictionary<string, object> 
                            { 
                                { "enable", true }
                                { "value", 42 }
                            });
    */
    public static void TriggerEvent(string eventName, Dictionary<string, object> message)
    {
        Action<Dictionary<string, object>> ev = null;
        if (instance.eventDict.TryGetValue(eventName, out ev))
        {
            ev.Invoke(message);
        }
    }
}
