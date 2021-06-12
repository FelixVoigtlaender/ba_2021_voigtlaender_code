using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisVariable : VisLogicElement
{
    VRVariable vrVariable;
    public Slider slider;
    public override void Setup(VRLogicElement element)
    {
        this.vrVariable = (VRVariable)element;
        base.Setup(element);

        textName.text = vrVariable.vrData.GetName();
        HandleTypes(vrVariable.vrData);
    }
    public override bool IsType(VRLogicElement vrLogicElement)
    {
        return vrLogicElement is VRVariable;
    }

    public void HandleTypes(VRData data)
    {
        switch (data)
        {
            case DatFloat datFloat :
                slider.gameObject.SetActive(true);
                slider.value = datFloat.value;
                slider.onValueChanged.AddListener(value => { datFloat.value = value; textName.text = datFloat.GetName(); });
                break;
            default:
                break;
        }
    }
}
