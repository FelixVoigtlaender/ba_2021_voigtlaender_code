using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisVariable : VisLogicElement
{
    public override bool IsType(VRLogicElement vrLogicElement)
    {
        return vrLogicElement is VRVariable;
    }
}
