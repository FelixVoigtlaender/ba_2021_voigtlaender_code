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

    public bool blockPorts = false;
    public bool blockInputs = false;
    public bool blockOutputs = false;

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
    public VRVariable(VRData vrData, string name = "", bool blockPorts = false, bool blockInputs = false, bool blockOutputs = false)
    {
        this.blockPorts = blockPorts;
        this.blockInputs = blockInputs;
        this.blockOutputs = blockOutputs;

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
        if (blockPorts || blockOutputs)
            return;


        output = new VRPort(GetData, vrData);
        if(name.Length != 0)
            output.toolTip = "Get " + name;
        vrOutputs.Add(output);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();

        if (blockPorts || blockInputs)
            return;

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
