using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VRLogicElement 
{
    public List<VRPort> inputs;
    public List<VRPort> outputs;

    public abstract string Name();
    public virtual void SetupPorts()
    {
        SetupInputs();
        SetupOutputs();
    }

    public virtual void SetupInputs()
    {
        inputs = new List<VRPort>();
    }

    public virtual void SetupOutputs()
    {
        outputs = new List<VRPort>();
    }

    public virtual void Trigger()
    {
    }
}
