using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisEvent : VisLogicElement
{
    public VREvent vrEvent;
    public string eventName;
    public override bool IsType(VRLogicElement vrLogicElement)
    {
        return vrLogicElement is VREvent;
    }

    public override void Init()
    {
        if (vrEvent != null)
            return;

        vrEvent = VRManager.instance.InitVREvent(eventName,false);
        Setup(vrEvent);
    }
}
