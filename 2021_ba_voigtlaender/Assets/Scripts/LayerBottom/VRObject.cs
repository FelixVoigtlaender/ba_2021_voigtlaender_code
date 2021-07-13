using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class VRObject
{
    public List<VRProperty> properties = new List<VRProperty>();
    public List<VREvent> vrEvents = new List<VREvent>();
    public GameObject gameObject;
    public event Action OnDelete;
    public Rigidbody rigid;


    public void Setup(GameObject gameObject)
    {
        this.gameObject = gameObject;
        rigid = gameObject.GetComponent<Rigidbody>();
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
        foreach(VRProperty property in properties)
        {
            property.Trigger();
        }
    }

    public void Delete()
    {
        foreach(VRProperty prop in properties)
        {
            prop?.Delete();
        }
        foreach (VREvent vrEvent in vrEvents)
        {
            vrEvent?.Delete();
        }
        OnDelete?.Invoke();
    }
}
