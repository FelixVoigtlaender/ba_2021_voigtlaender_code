using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public class VRVariable : VRLogicElement
{
    string name = "";
    public VRData vrData;
    protected VRPort output;
    protected VRPort input;

    public event Action<VRData> OnVariableChanged;
    public override string Name()
    {
        return name;
    }

    public void Setup(VRData vrData)
    {
        this.vrData = vrData;
        base.Setup();

        OnVariableChanged?.Invoke(vrData);
    }
    public override void SetupOutputs()
    {
        base.SetupOutputs();
        output = new VRPort(GetData, vrData);
        vrOutputs.Add(output);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        input = new VRPort(this, vrData);
        vrInputs.Add(input);
    }

    public VRData GetData()
    {
        if (input.IsConnected())
        {
            vrData.SetData(input.GetData());
            OnVariableChanged?.Invoke(vrData);
        }

        return vrData;
    }
}
