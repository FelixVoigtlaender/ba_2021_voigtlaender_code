using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisTab : VisLogicElement
{
    public override bool IsType(VRLogicElement vrLogicElement)
    {
        return vrLogicElement is VRTab;
    }

}
