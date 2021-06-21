using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VRAction : VRLogicElement
{
    public override void Setup()
    {
        base.Setup();
    }

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
        base.SetupVariables();
        DatVector3 datPosition = new DatVector3(Vector3.zero);
        varPosition = new VRVariable();
        varPosition.Setup(datPosition);
        vrVariables.Add(varPosition);
    }

    public override void SetupInputs()
    {
        base.SetupInputs();

        inTrigger = new VRPort(SetData, new DatEvent(0f));
        vrInputs.Add(inTrigger);

        inObject = new VRPort(this, new DatObj(null));
        vrInputs.Add(inObject);
    }

    public override void SetupOutputs()
    {
        base.SetupOutputs();
        outTrigger = new VRPort(GetElementData, new DatEvent(0));
        vrOutputs.Add(outTrigger);
    }

    public void SetData(VRData datEvent)
    {
        VRDebug.SetLog($"{Name()}: TRIGGERED");

        if (!inObject.IsConnected())
            return;

        DatObj datObj = (DatObj) inObject.GetData();
        DatVector3 datVector3 =(DatVector3) varPosition.GetData();
        datObj.Value.gameObject.transform.position = datVector3.Value;

        if (outTrigger.IsConnected())
            outTrigger.SetData(datEvent);
    }

    public VRData GetElementData()
    {
        return new DatEvent(-1);
    }
}