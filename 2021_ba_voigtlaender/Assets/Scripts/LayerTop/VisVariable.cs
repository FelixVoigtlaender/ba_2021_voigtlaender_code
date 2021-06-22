using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class VisVariable : VisLogicElement
{
    VRVariable vrVariable;
    public Slider slider;
    public Button button;
    VisVector visVector;
    public override void Setup(VRLogicElement element)
    {
        this.vrVariable = (VRVariable)element;
        base.Setup(element);

        textName.text = vrVariable.vrData.GetName();
        SetupTypes(vrVariable.vrData);

        vrVariable.vrData.OnDataChanged += OnDataChanged;
    }

    public void SetupTypes(VRData data)
    {
        switch (data)
        {
            case DatFloat datFloat:
                slider.gameObject.SetActive(true);
                slider.value = datFloat.Value;
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener(value => { datFloat.Value = value; textName.text = datFloat.GetName(); });
                break;
            case DatVector3 datVector:
                if (button)
                {
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(ButtonPressed);
                }
                break;
            default:
                break;
        }
    }

    public void OnDataChanged(VRData vrData)
    {
        if(textName)
            textName.text = vrData.GetName();
    }

    private void Update()
    {
        if (vrVariable == null || vrVariable.vrData == null)
            return;
        HandleTypes(vrVariable.vrData);
    }
    public void HandleTypes(VRData data)
    {
        switch (data)
        {
            case DatFloat datFloat:
                slider.value = datFloat.Value;
                break;
            case DatVector3 datVector:
                if (visVector != null && visVector.transform)
                    datVector.Value = visVector.transform.position;
                break;
            default:
                break;
        }
    }

    public void ButtonPressed()
    {
        if (vrVariable == null || vrVariable.vrData == null)
            return;
        switch (vrVariable.vrData)
        {
            case DatFloat datFloat:
                slider.value = datFloat.Value;
                break;
            case DatVector3 datVector:
                if(visVector!=null && visVector.transform && visVector.transform.gameObject.activeSelf)
                {
                    visVector.transform.gameObject.SetActive(false);
                }
                else
                {
                    visVector = VisManager.instance.DemandVisVector();
                    Vector3 position = datVector.Value.magnitude > 0.1f ? datVector.Value : transform.position + Vector3.up * 0.2f;
                    visVector.transform.position = transform.position;
                    visVector.transform.DOMove(position, 0.2f);
                }
                break;
            default:
                break;
        }
    }

    public override void OnDelete()
    {
        if (visVector != null && visVector.transform)
            visVector.transform.gameObject.SetActive(false);
        base.OnDelete();
    }
    public override bool IsType(VRLogicElement vrLogicElement)
    {
        return vrLogicElement is VRVariable;
    }
}
