using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public class VRTab : VRLogicElement
{
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
            if(!isActive)
                Detach();
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
