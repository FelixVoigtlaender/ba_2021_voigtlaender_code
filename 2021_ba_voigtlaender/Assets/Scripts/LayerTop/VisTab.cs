using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisTab : VisLogicElement
{
    VRTab vrTab;
    Toggle toggle;
    public override bool IsType(VRLogicElement vrLogicElement)
    {
        return vrLogicElement is VRTab;
    }


    public override void Setup(VRLogicElement element)
    {
        base.Setup(element);
        this.vrTab = (VRTab)element;

        OnIsActiveChanged(vrTab.IsActive);
    }

    public void OnIsActiveChanged(bool value)
    {
        gameObject.SetActive(value);
        vrTab.IsActive = value;
        if(toggle)
            toggle.SetIsOnWithoutNotify(value);
    }
    public void SetToggle(Toggle toggle)
    {
        this.toggle = toggle;
        toggle.onValueChanged.AddListener(OnIsActiveChanged);
        OnIsActiveChanged(vrTab.IsActive);
    }
}
