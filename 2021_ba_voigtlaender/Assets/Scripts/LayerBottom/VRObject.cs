using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRObject : MonoBehaviour
{
    public List<VRProperty> properties;
    public List<VREvent> vrEvents;




    public void Trigger()
    {
        foreach(VREvent vrEvent in vrEvents)
        {
            vrEvent.Trigger();
        }
    }
}
