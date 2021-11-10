using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class VRObject : SaveElement
{
    [SerializeReference] public List<VRProperty> properties = new List<VRProperty>();
    [SerializeReference] public List<VRPort> vrInputs = new List<VRPort>();
    [SerializeReference] public List<VRPort> vrOutputs = new List<VRPort>();
    [SerializeReference] public GameObject gameObject;
    [SerializeReference] public Rigidbody rigid;

    public VRObject()
    {
        isRoot = true;
        Debug.Log("WHO THE FUCK CALLS ME");
    }

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

    public override void Delete()
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

        base.Delete();
    }
}
