using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class VRLogicElement 
{
    public List<VRPort> vrInputs = new List<VRPort>();
    public List<VRPort> vrOutputs = new List<VRPort>();
    public List<VRVariable> vrVariables = new List<VRVariable>();
    public List<VRTab> vrTabs = new List<VRTab>();

    public event Action OnDelete; 
    public abstract string Name();

    public virtual void Setup()
    {
        OnDelete = null;
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

    public virtual void Delete()
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

        OnDelete?.Invoke();
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
