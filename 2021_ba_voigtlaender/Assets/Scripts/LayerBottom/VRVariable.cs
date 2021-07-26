using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public class VRVariable : VRLogicElement
{
    public string name = "";
    public bool allowDatName = false;
    public VRData vrData;
    protected VRPort output;
    protected VRPort input;

    public event Action<VRData> OnVariableChanged;
    public event Action<VRData> OnSetData;
    public event Func<VRData> OnGetData;
    public override string Name()
    {
        if (name.Length > 0)
        {
            return allowDatName ? name + " " + vrData.GetName() : name;

        }
        return vrData.GetName();
    }
    public VRVariable() { }
    public VRVariable(VRData vrData, string name = "")
    {
        Setup(vrData, name);
    }
    public void Setup(VRData vrData)
    {
        this.vrData = vrData;
        base.Setup();

        OnVariableChanged?.Invoke(vrData);
    }
    public void Setup(VRData vrData, string name)
    {
        this.name = name;
        Setup(vrData);
    }
    public override void SetupOutputs()
    {
        base.SetupOutputs();
        output = new VRPort(GetData, vrData);
        if(name.Length != 0)
            output.toolTip = "Get " + name;
        vrOutputs.Add(output);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        input = new VRPort(SetData, vrData);
        input.OnConnect += () => GetData();
        if (name.Length != 0)
            input.toolTip = "Set " + name;
        vrInputs.Add(input);
    }

    public VRData GetData()
    {
        if (input.IsConnected())
        {
            vrData.SetData(input.GetData());
            OnVariableChanged?.Invoke(vrData);
        }
        else if (OnGetData != null)
        {
            vrData = OnGetData();
        }

        return vrData;
    }
    public void SetData(VRData vrData)
    {
        vrData.SetData(vrData);
        OnVariableChanged?.Invoke(vrData);
        output.SetData(vrData);
        OnSetData?.Invoke(vrData);
    }
}
