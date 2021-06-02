using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRObject : MonoBehaviour
{
    public List<VRProperty> properties;
    public List<VREvent> vrEvents;

    private void Start()
    {
        
    }


    public void Setup()
    {
        SetupProperties();
    }

    public void SetupProperties()
    {
        properties = new List<VRProperty>();
        List<VRProperty> possibleProperties = VRProperty.GetAllPorperties();
        foreach(VRProperty vRProperty in possibleProperties)
        {
            if (!vRProperty.IsType(this))
                continue;

            vRProperty.Setup(this);
            properties.Add(vRProperty);
        }
    }

    public void Trigger()
    {
        foreach(VREvent vrEvent in vrEvents)
        {
            vrEvent.Trigger();
        }
    }
}
