using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class VRObject
{
    public List<VRProperty> properties = new List<VRProperty>();
    public List<VRPort> vrInputs = new List<VRPort>();
    public List<VRPort> vrOutputs = new List<VRPort>();
    public GameObject gameObject;
    public event Action OnDelete;
    public Rigidbody rigid;


    public void Setup(GameObject gameObject)
    {
        this.gameObject = gameObject;
        rigid = gameObject.GetComponent<Rigidbody>();
        SetupProperties();
        SetupPorts();
    }

    public void SetupProperties()
    {
        properties = new List<VRProperty>();
        List<VRProperty> possibleProperties = VRProperty.GetAllPorperties();
        foreach(VRProperty vrProperty in possibleProperties)
        {
            if (!vrProperty.IsType(this))
                continue;

            VRProperty vrPropertyClone = (VRProperty) vrProperty.CreateInstance();

            vrPropertyClone.Setup(this);
            properties.Add(vrPropertyClone);
        }
    }
    public virtual void SetupPorts()
    {
        SetupInputs();
        SetupOutputs();
    }

    public virtual void SetupInputs()
    {
        vrInputs = new List<VRPort>();
    }

    public virtual void SetupOutputs()
    {
        vrOutputs = new List<VRPort>();
        vrOutputs.Add(new VRPort(GetData,new DatObj(this)));
    }

    public VRData GetData()
    {
        return new DatObj(this);
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
        foreach (VRPort input in vrInputs)
        {
            input?.Delete();
        }
        foreach (VRPort output in vrOutputs)
        {
            output?.Delete();
        }
        OnDelete?.Invoke();
    }
}
