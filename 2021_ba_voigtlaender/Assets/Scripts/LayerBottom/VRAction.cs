using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VRAction : VRLogicElement
{
    public List<VRVariable> vrVariables = new List<VRVariable>();

    public override void Setup()
    {
        SetupVariables();
        base.Setup();
    }
    public abstract void SetupVariables();

    public static List<VRAction> GetAllActions()
    {
        List<VRAction> allEvents = new List<VRAction>();
        allEvents.Add(new ActPosition());
        return allEvents;
    }

    public static VRAction GetAction(string name)
    {
        List<VRAction> allActions = GetAllActions();
        foreach (VRAction vrAction in allActions)
        {
            if (vrAction.Name() == name)
                return vrAction;
        }

        return null;
    }
}


public class ActPosition : VRAction
{
    VRVariable varPosition;

    VRPort inTrigger;
    VRPort inObject;

    VRPort outTrigger;

    public override string Name()
    {
        return "Set Position";
    }

    public override void SetupVariables()
    {
        DatVector3 datPosition = new DatVector3(Vector3.zero);
        varPosition = new VRVariable();
        varPosition.Setup(datPosition);
        vrVariables.Add(varPosition);
    }

    public override void SetupInputs()
    {
        base.SetupInputs();

        inTrigger = new VRPort(SetData, new DatEvent(0f));
        inputs.Add(inTrigger);

        inObject = new VRPort(this, new DatObj(null));
        inputs.Add(inObject);
    }

    public override void SetupOutputs()
    {
        base.SetupOutputs();
        outTrigger = new VRPort(GetElementData, new DatEvent(0));
        outputs.Add(outTrigger);
    }

    public void SetData(VRData datEvent)
    {
        VRDebug.SetLog($"{Name()}: TRIGGERED");

        if (!inObject.IsConnected())
            return;

        DatObj datObj = (DatObj) inObject.GetData();
        DatVector3 datVector3 =(DatVector3) varPosition.GetData();
        datObj.value.gameObject.transform.position = datVector3.value;

        if (outTrigger.IsConnected())
            outTrigger.SetData(datEvent);
    }

    public VRData GetElementData()
    {
        return new DatEvent(-1);
    }
}