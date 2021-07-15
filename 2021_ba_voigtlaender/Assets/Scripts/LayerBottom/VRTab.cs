using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VRTab : VRLogicElement
{
    bool isActive = false;
    public event Action<bool> OnIsActiveChanged;
    public bool IsActive
    {
        get 
        {
            return isActive;
        }
        set
        {
            isActive = value;
            OnIsActiveChanged?.Invoke(value);
        }
    }
    public string name = "UNASSIGNED";


    public VRTab(string name = "UNASSIGNED")
    {
        Setup(name);
    }
    public void Setup(string name)
    {
        this.name = name;
        base.Setup();
    }

    public override string Name()
    {
        return name;
    }
}
