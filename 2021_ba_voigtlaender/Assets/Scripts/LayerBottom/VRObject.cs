using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRObject
{
    public List<VRProperty> properties;
    public List<VREvent> vrEvents;
    public GameObject gameObject;

    public void Setup(GameObject gameObject)
    {
        this.gameObject = gameObject;
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
