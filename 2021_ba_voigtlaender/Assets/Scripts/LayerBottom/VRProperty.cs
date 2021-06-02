using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VRProperty : VRLogicElement
{
    protected VRObject vrObject;
    protected VRPort output;
    protected VRPort input;

    public abstract VRData GetData();
    
    public abstract bool IsType(VRObject vrObject);
    public virtual void Setup(VRObject vrObject) 
    {
        SetupPorts();
    }

    public static List<VRProperty> GetAllPorperties()
    {
        List<VRProperty> allProperties = new List<VRProperty>();
        allProperties.Add(new PropPosition());
        allProperties.Add(new PropScale());
        return allProperties;
    }
}

public class PropPosition : VRProperty
{
    public override string Name()
    {
        return "Position";
    }
    public override bool IsType(VRObject vrObject)
    {
        // Gameobject always has a transform
        return true;
    }

    public override void SetupOutputs()
    {
        base.SetupOutputs();
        output = new VRPort(GetData, new VRVector3(Vector3.zero));
        outputs.Add(output);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        input = new VRPort(this, new VRVector3(Vector3.zero));
        inputs.Add(input);
    }

    public override VRData GetData()
    {
        return new VRVector3(vrObject.transform.position);
    }

    public override void Trigger()
    {
        if (!input.IsConnected())
            return;

        VRVector3 vrVector3 = (VRVector3)input.GetData();
        vrObject.transform.position = vrVector3.value;
    }

}

public class PropScale : VRProperty
{

    public override string Name()
    {
        return "Scale";
    }
    public override bool IsType(VRObject vrObject)
    {
        // Gameobject always has a transform
        return true;
    }

    public override void SetupOutputs()
    {
        base.SetupOutputs();
        output = new VRPort(GetData, new VRFloat(0));
        outputs.Add(output);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        input = new VRPort(this, new VRFloat(0));
        inputs.Add(input);
    }

    public override VRData GetData()
    {
        return new VRFloat(vrObject.transform.localScale.x);
    }

    public override void Trigger()
    {
        if (!input.IsConnected())
            return;

        VRFloat vrFloat = (VRFloat)input.GetData();
        vrObject.transform.localScale = Vector3.one * vrFloat.value;
    }
}