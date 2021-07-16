using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class VREvent : VRLogicElement
{
    static List<VREvent> allEvents;
    public virtual void Update(DatEvent vREventDat)
    {

    }
    public static List<VREvent> GetAllEvents()
    {
        if (allEvents != null)
            return allEvents;


        allEvents = new List<VREvent>();
        IEnumerable<Type> subClasses = VRManager.GetAllSubclassOf(typeof(VREvent));
        foreach (Type type in subClasses)
        {
            VREvent obj = (VREvent)Activator.CreateInstance(type);
            allEvents.Add(obj);
        }
        return allEvents;
    }

    public static VREvent GetEvent(string name)
    {
        List<VREvent> allEvents = GetAllEvents();
        foreach(VREvent vrEvent in allEvents)
        {
            if (vrEvent.Name() == name)
                return vrEvent;
        }

        return null;
    }
}


[System.Serializable]
public class EventProximity : VREvent
{
    VRPort outEvent;
    VRPort inEvent;


    VRVariable varDistance;
    VRVariable varObjA;
    VRVariable varObjB;

    public override string Name()
    {
        return "ProximityAlert";
    }
    public override void Setup()
    {
        base.Setup();
        VRManager.instance.OnFixedUpdate += Update;
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        inEvent = new VRPort(SetData, new DatEvent(0f));
        vrInputs.Add(inEvent);
    }
    public override void SetupOutputs()
    {
        base.SetupOutputs();
        outEvent = new VRPort(GetData, new DatEvent(0f));
        vrOutputs.Add(outEvent);
    }
    public override void SetupVariables()
    {
        base.SetupVariables();

        varDistance = new VRVariable();
        varDistance.Setup(new DatFloat(0));
        vrVariables.Add(varDistance);

        varObjA = new VRVariable();
        varObjA.Setup(new DatObj(null));
        vrVariables.Add(varObjA);

        varObjB = new VRVariable();
        varObjB.Setup(new DatObj(null));
        vrVariables.Add(varObjB);
    }

    public override void Update(DatEvent datEvent)
    {
        if (inEvent.IsConnected())
            return;

        SetData(datEvent);
    }

    public void SetData(VRData vrData)
    {
        DatEvent datEvent = (DatEvent)vrData;

        VRDebug.SetLog($"{Name()}: UPDATE {datEvent.Value.ToString("0.0")}");

        DatObj datObjA = (DatObj)varObjA.GetData();
        DatObj datObjB = (DatObj)varObjB.GetData();
        DatFloat datDistance = (DatFloat)varDistance.GetData();


        VRDebug.SetLog($"{Name()}: {datObjA.Value != null} {datObjB.Value != null} {datDistance.Value}");
        if (datObjA.Value == null || datObjB.Value == null)
            return;

        Vector3 posA = datObjA.Value.gameObject.transform.position;
        Vector3 posB = datObjB.Value.gameObject.transform.position;
        float distance = datDistance.Value;

        if (Vector3.Distance(posA, posB) < distance)
        {
            outEvent.SetData(datEvent);
            VRDebug.SetLog($"{Name()}: TRIGGERED");
        }
        else
        {
            VRDebug.SetLog($"{Name()}: NOT-TRIGGERED");
        }
    }
    public VRData GetData()
    {
        return null;
    }

    public override void Delete()
    {
        VRManager.instance.OnFixedUpdate -= Update;

        base.Delete();
    }
}

[System.Serializable]
public class EventRandom : VREvent
{
    VRPort outEvent;


    VRVariable varDistance;
    VRVariable varObjA;
    VRVariable varObjB;

    public override string Name()
    {
        return "RandomEvent";
    }

    public override void SetupOutputs()
    {
        base.SetupOutputs();
        outEvent = new VRPort(GetData, new DatEvent(0f));
        vrOutputs.Add(outEvent);
    }
    public override void SetupVariables()
    {
        base.SetupVariables();

        varDistance = new VRVariable();
        varDistance.Setup(new DatFloat(0));
        //vrVariables.Add(varDistance);

        varObjA = new VRVariable();
        varObjA.Setup(new DatObj(null));
        vrVariables.Add(varObjA);

        varObjB = new VRVariable();
        varObjB.Setup(new DatObj(null));
        vrVariables.Add(varObjB);
    }

    public override void Update(DatEvent vREventDat)
    {

        VRDebug.SetLog($"{Name()}: UPDATE {vREventDat.Value.ToString("0.0")}");

        DatObj datObjA = (DatObj)varObjA.GetData();
        DatObj datObjB = (DatObj)varObjB.GetData();
        DatFloat datDistance = (DatFloat)varDistance.GetData();


        VRDebug.SetLog($"{Name()}: {datObjA.Value != null} {datObjB.Value != null} {datDistance.Value}");
        if (datObjA.Value == null || datObjB.Value == null)
            return;

        Vector3 posA = datObjA.Value.gameObject.transform.position;
        Vector3 posB = datObjB.Value.gameObject.transform.position;
        float distance = datDistance.Value;

        if (Vector3.Distance(posA, posB) < distance)
        {
            outEvent.SetData(vREventDat);
            VRDebug.SetLog($"{Name()}: TRIGGERED");
        }
        else
        {
            VRDebug.SetLog($"{Name()}: NOT-TRIGGERED");
        }

    }

    public VRData GetData()
    {
        return null;
    }
}
