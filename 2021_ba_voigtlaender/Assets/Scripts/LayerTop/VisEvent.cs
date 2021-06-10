using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisEvent : VisLogicElement
{
    public override bool IsType(VRLogicElement vrLogicElement)
    {
        return vrLogicElement is VREvent;
    }
}
