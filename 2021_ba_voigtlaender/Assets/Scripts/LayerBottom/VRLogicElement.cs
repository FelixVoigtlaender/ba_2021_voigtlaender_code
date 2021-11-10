using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class VRLogicElement : SaveElement
{
    
    [SerializeReference] public List<VRPort> vrInputs = new List<VRPort>();
    [SerializeReference] public List<VRPort> vrOutputs = new List<VRPort>();
    [SerializeReference] public List<VRVariable> vrVariables = new List<VRVariable>();
    [SerializeReference] public List<VRTab> vrTabs = new List<VRTab>();

    public event Action<bool> OnActiveChanged;
    [SerializeField] protected bool _isActive = true;
    public bool isActive
    {
        get { return _isActive; }
        set
        {
            if(_isActive != value)
                OnActiveChanged?.Invoke(value);
            _isActive = value;
        }

    }

    public abstract string Name();

    public virtual void Setup()
    {
        SetupPorts();
        SetupVariables();
        SetupTabs();
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

    public virtual void SetupVariables()
    {
        vrVariables = new List<VRVariable>();
    }
    public virtual void SetupTabs()
    {
        vrTabs = new List<VRTab>();
    }
    public virtual void SetTabActive(int index)
    {
        for (int i = 0; i < vrTabs.Count; i++)
        {
            vrTabs[i].IsActive = index == i;
        }
    }
    public virtual VRTab GetActiveTab()
    {
        foreach(VRTab vrTab in vrTabs)
        {
            if (vrTab.IsActive)
                return vrTab;
        }
        return null;
    }

    public virtual bool VariablesCheck()
    {
        foreach(VRVariable variable in vrVariables)
        {
            if (variable.GetData() == null)
                return false;
        }
        return true;
    }

    public virtual void Trigger()
    {
    }

    public override void Delete()
    {
        VRDebug.Log("DELETING: " + this.ToString());
        foreach(VRPort input in vrInputs)
        {
            input?.Delete();
        }
        foreach (VRPort output in vrOutputs)
        {
            output?.Delete();
        }
        foreach (VRVariable variable in vrVariables)
        {
            variable?.Delete();
        }
        foreach(VRTab tab in vrTabs)
        {
            tab?.Delete();
        }
        base.Delete();
    }

    public virtual void Detach()
    {

        VRDebug.Log("Detaching: " + this.ToString());
        foreach (VRPort input in vrInputs)
        {
            input?.Detach();
        }
        foreach (VRPort output in vrOutputs)
        {
            output?.Detach();
        }
        foreach (VRVariable variable in vrVariables)
        {
            variable?.Detach();
        }
        foreach (VRTab tab in vrTabs)
        {
            tab?.Detach();
        }
    }

    public VRVariable FindVariable(string name)
    {
        foreach (VRVariable variable in vrVariables)
        {
            if (variable.Name() == name)
                return variable;
        }
        return null;
    }


    public List<VRVariable> FindVariables(VRData dataType)
    {
        List<VRVariable> variables = new List<VRVariable>();
        foreach (VRVariable variable in vrVariables)
        {
            if (variable.vrData.IsType(dataType))
                variables.Add(variable);
        }
        return variables;
    }
    public VRVariable FindVariable(VRData dataType)
    {
        foreach (VRVariable variable in vrVariables)
        {
            if (variable.vrData.IsType(dataType))
                return variable;
        }
        return null;
    }

    public VRLogicElement ShallowCopy()
    {
        return (VRLogicElement) this.MemberwiseClone();
    }
    public VRLogicElement CreateInstance()
    {
        return (VRLogicElement)Activator.CreateInstance(this.GetType());
    }

}
