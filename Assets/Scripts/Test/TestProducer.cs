using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProducer : MonoBehaviour
{   
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            EventManager.TriggerEvent("TestEvent", new Dictionary<string, object> { {"Value1", 42} });
            //EventManager.TriggerEvent("PlaySFX", new Dictionary<string, object> { { "SFXsName", "Cricket" } });
        }        
    }
}
