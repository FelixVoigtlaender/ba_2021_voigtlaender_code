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
        this.vrObject = vrObject;
        base.Setup();
    }

    public static List<VRProperty> GetAllPorperties()
    {
        List<VRProperty> allProperties = new List<VRProperty>();
        allProperties.Add(new PropObj());
        allProperties.Add(new PropPosition());
        allProperties.Add(new PropScale());
        return allProperties;
    }
}

public class PropObj : VRProperty
{
    public override string Name()
    {
        return "Object";
    }
    public override bool IsType(VRObject vrObject)
    {
        // Gameobject always has a transform
        return true;
    }

    public override void SetupOutputs()
    {
        base.SetupOutputs();
        output = new VRPort(GetData, new DatObj(new VRObject()));
        outputs.Add(output);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        input = new VRPort(this, new DatObj(new VRObject()));
        inputs.Add(input);
    }

    public override VRData GetData()
    {
        return new DatObj(vrObject);
    }

    public override void Trigger()
    {
        if (!input.IsConnected())
            return;

        DatObj data = (DatObj)input.GetData();
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
        output = new VRPort(GetData, new DatVector3(Vector3.zero));
        outputs.Add(output);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        input = new VRPort(this, new DatVector3(Vector3.zero));
        inputs.Add(input);
    }

    public override VRData GetData()
    {
        return new DatVector3(vrObject.gameObject.transform.position);
    }

    public override void Trigger()
    {
        if (!input.IsConnected())
            return;

        DatVector3 vrVector3 = (DatVector3)input.GetData();
        vrObject.gameObject.transform.position = vrVector3.value;
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
        output = new VRPort(GetData, new DatFloat(0));
        outputs.Add(output);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        input = new VRPort(this, new DatFloat(0));
        inputs.Add(input);
    }

    public override VRData GetData()
    {
        return new DatFloat(vrObject.gameObject.transform.localScale.x);
    }

    public override void Trigger()
    {
        if (!input.IsConnected())
            return;

        DatFloat vrFloat = (DatFloat)input.GetData();
        vrObject.gameObject.transform.localScale = Vector3.one * vrFloat.value;
    }
}