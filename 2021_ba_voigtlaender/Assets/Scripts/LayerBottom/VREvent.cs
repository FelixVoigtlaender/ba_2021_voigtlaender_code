using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


public class EventDistance : VREvent
{
    VRPort outEvent;

    VRPort inDistance;
    VRPort inObjA;
    VRPort inObjB;

    public override string Name()
    {
        return "Distance";
    }

    public override void SetupOutputs()
    {
        base.SetupOutputs();
        outEvent = new VRPort(GetData, new DatEvent(0f));
        outputs.Add(outEvent);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        inDistance = new VRPort(this, new DatFloat(0));
        inputs.Add(inDistance);

        inObjA = new VRPort(this, new DatObj(null));
        inputs.Add(inObjA);

        inObjB = new VRPort(this, new DatObj(null));
        inputs.Add(inObjB);
    }

    public override void Update(DatEvent vREventDat)
    {
        if (!inObjA.IsConnected() || !inObjB.IsConnected() || !inDistance.IsConnected())
            return;

        DatObj datObjA =(DatObj) inObjA.GetData();
        DatObj datObjB = (DatObj)inObjB.GetData();
        DatFloat datDistance = (DatFloat)inDistance.GetData();


        Vector3 posA = datObjA.value.gameObject.transform.position;
        Vector3 posB = datObjB.value.gameObject.transform.position;
        float distance = datDistance.value;

        if (Vector3.Distance(posA, posB) < distance)
            outEvent.SetData(vREventDat);

    }

    public VRData GetData()
    {
        return null;
    }
}
