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

        vrVariable.vrData.OnDataChanged += OnDataChanged;
    }
    public override bool IsType(VRLogicElement vrLogicElement)
    {
        return vrLogicElement is VRVariable;
    }

    public void OnDataChanged(VRData vrData)
    {
        textName.text = vrData.GetName();
    }

    public void HandleTypes(VRData data)
    {
        switch (data)
        {
            case DatFloat datFloat :
                slider.gameObject.SetActive(true);
                slider.value = datFloat.Value;
                slider.onValueChanged.AddListener(value => { datFloat.Value = value; textName.text = datFloat.GetName(); });
                break;
            default:
                break;
        }
    }
}
