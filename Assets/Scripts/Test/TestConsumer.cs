using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestConsumer : MonoBehaviour
{    
    private void OnEnable() 
    {
        EventManager.StartListening("TestEvent", OnTestFunction);
    }

    private void OnDisable() 
    {
        EventManager.StopListening("TestEvent", OnTestFunction);
    }

    private void OnDestroy() 
    {
        EventManager.StopListening("TestEvent", OnTestFunction);
        Destroy(gameObject);
        Debug.Log("Destroyed");
    }
    
    void OnTestFunction(Dictionary<string, object> message)
    {
        var p1 = (int)message["Value1"];
        Debug.Log($"OnTestFunction called: Value1={p1}");
    }
}
