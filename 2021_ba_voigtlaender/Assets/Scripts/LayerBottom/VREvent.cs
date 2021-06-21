using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class VREvent : VRLogicElement
{
    public virtual void Update(DatEvent vREventDat)
    {

    }

    public static List<VREvent> GetAllEvents()
    {
        List<VREvent> allEvents = new List<VREvent>();
        allEvents.Add(new EventDistance());
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
public class EventDistance : VREvent
{
    VRPort outEvent;


    VRVariable varDistance;
    VRVariable varObjA;
    VRVariable varObjB;

    public override string Name()
    {
        return "Distance";
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

    public override void Update(DatEvent vREventDat)
    {

        VRDebug.SetLog($"{Name()}: UPDATE {vREventDat.Value.ToString("0.0")}");

        DatObj datObjA =(DatObj) varObjA.GetData();
        DatObj datObjB = (DatObj) varObjB.GetData();
        DatFloat datDistance = (DatFloat)varDistance.GetData();


        VRDebug.SetLog($"{Name()}: {datObjA.Value!=null} {datObjB.Value != null} {datDistance.Value}");
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
