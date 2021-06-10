using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRVariable : VRLogicElement
{
    string name = "";
    public VRData vrData;
    protected VRPort output;
    protected VRPort input;
    public override string Name()
    {
        return name;
    }

    public void Setup(VRData vrData)
    {
        this.vrData = vrData;
        SetupPorts();
    }
    public override void SetupOutputs()
    {
        base.SetupOutputs();
        output = new VRPort(GetData, vrData);
        outputs.Add(output);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        input = new VRPort(this, vrData);
        inputs.Add(input);
    }

    public VRData GetData()
    {
        return vrData;
    }
}
