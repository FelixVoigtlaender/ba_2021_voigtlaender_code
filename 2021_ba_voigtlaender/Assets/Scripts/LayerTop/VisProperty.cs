using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisProperty : VisLogicElement
{
    public override bool IsType(VRLogicElement vrLogicElement)
    {
        return vrLogicElement is VRProperty;
    }

}
