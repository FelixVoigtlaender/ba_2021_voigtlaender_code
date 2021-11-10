using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
public abstract class VRAction : VRLogicElement
{

    static List<VRAction> allActions;

    public VRAction()
    {
        isRoot = true;
    }


    public static List<VRAction> GetAllActions()
    {
        if (allActions != null)
            return allActions;


        allActions = new List<VRAction>();
        IEnumerable<Type> subClasses = VRManager.GetAllSubclassOf(typeof(VRAction));
        foreach (Type type in subClasses)
        {
            VRAction obj = (VRAction)Activator.CreateInstance(type);
            allActions.Add(obj);
        }
        return allActions;
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


public class ActMove : VRAction
{
    VRVariable varPosition;
    VRVariable varObject;
    VRVariable varDuration;

    VRPort inTrigger;

    public override string Name()
    {
        return "Move";
    }

    public override void SetupVariables()
    {
        base.SetupVariables();


        DatObj datObj = new DatObj(new VRObject());
        varObject = new VRVariable();
        varObject.Setup(datObj);
        vrVariables.Add(varObject);

        DatVector3 datPosition = new DatVector3(Vector3.zero);
        varPosition = new VRVariable();
        varPosition.Setup(datPosition);
        vrVariables.Add(varPosition);

        DatFloat datFloat = new DatFloat(1);
        datFloat.max = 3;
        varDuration = new VRVariable();
        varDuration.Setup(datFloat);
        varDuration.allowDatName = true;
        varDuration.name = "Duration";
        vrVariables.Add(varDuration);
    }

    public override void SetupInputs()
    {
        base.SetupInputs();

        inTrigger = new VRPort(this, new DatEvent(0f), PortType.INPUT);
        vrInputs.Add(inTrigger);
    }

    public override void SetupOutputs()
    {
        base.SetupOutputs();
    }

    public override void SetData(VRData datEvent)
    {
        VRDebug.SetLog($"{Name()}: TRIGGERED");

        DatObj datObj = (DatObj)varObject.GetData();
        DatFloat duration = (DatFloat)varDuration.GetData();
        DatVector3 position = (DatVector3)varPosition.GetData();

        if (datObj.Value == null ||datObj.Value.gameObject==null)
            return;

        if(datObj.Value.rigid)
            datObj.Value.gameObject.transform.DOMove(position.Value, duration.Value).OnUpdate(()=>datObj.Value.rigid.velocity = Vector3.zero);
        else
            datObj.Value.gameObject.transform.DOMove(position.Value, duration.Value);
    }

}