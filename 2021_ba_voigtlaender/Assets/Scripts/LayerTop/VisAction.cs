using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisAction : VisLogicElement
{
    public VRAction vrAction;
    public Transform variableHolder;
    public string actionName;
    public override bool IsType(VRLogicElement vrLogicElement)
    {
        return vrLogicElement is VRAction;
    }

    public override void Init()
    {
        if (vrAction != null)
            return;

        vrAction = VRManager.instance.InitVRAction(actionName);
        Setup(vrAction);
        PopulateVisVariables();
    }


    public void PopulateVisVariables()
    {
        foreach(VRVariable vrVariable in vrAction.vrVariables)
        {
            GameObject objVisVariable = Instantiate(VisManager.instance.GetVisLogicPrefab(vrVariable),variableHolder);
            VisVariable visVariable = objVisVariable.GetComponent<VisVariable>();
            visVariable.Setup(vrVariable);
        }
    }

}
