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
    public BetterToggle toggle;
    VisVector visVector;
    GhostObject ghostObject;


    public BetterToggle betterTogglePlay;
    public BetterToggle betterToggleRecord;

    public BetterColorPicker colorPicker;

    public override void Setup(VRLogicElement element)
    {
        this.vrVariable = (VRVariable)element;
        base.Setup(element);

        textName.text = vrVariable.Name();
        SetupTypes(vrVariable.vrData);

        vrVariable.vrData.OnDataChanged += OnDataChanged;
    }

    public void SetupTypes(VRData data)
    {
        switch (data)
        {
            case DatFloat datFloat:
                slider.gameObject.SetActive(true);
                if (datFloat.useMinMax)
                {
                    slider.minValue = datFloat.min;
                    slider.maxValue = datFloat.max;
                }
                slider.value = datFloat.Value;
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener(value => { datFloat.Value = value; textName.text = vrVariable.Name(); });
                break;
            case DatVector3 datVector:
                if (button)
                {
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(()=> 
                    {
                        if (visVector != null && visVector.transform && visVector.transform.gameObject.activeSelf)
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
                    });
                }
                break;
            case DatEvent datEvent:
                if (button)
                {
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(() =>
                    {
                        vrVariable.SetData(new DatEvent(VRManager.tickIndex));
                    });
                }
                break;
            case DatTransform datTransform:
                if (button)
                {
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(() =>
                    {
                        if (ghostObject != null && ghostObject.datTransform == datTransform && ghostObject.gameObject.activeSelf)
                        {
                            ghostObject.gameObject.SetActive(false);
                        }
                        else
                        {
                            ghostObject = VisManager.instance.DemandGhostObject(datTransform);
                        }
                    });
                }
                break;
            case DatBool datBool:
                if (toggle)
                {
                    toggle.gameObject.SetActive(true);
                    toggle.IsOn = datBool.Value;
                    toggle.OnValueChanged.AddListener((value) =>
                    {
                        datBool.Value = value; textName.text = vrVariable.Name();
                    });
                }
                break;
            case DatRecording datRecording:
                if (!button || !betterTogglePlay || !betterToggleRecord)
                    return;

                // Setup recording button
                betterToggleRecord.OnValueChanged.AddListener((value) =>
                {
                    if (!ghostObject)
                        return;
                    ghostObject.Record(value,()=> 
                    {
                        betterToggleRecord.SetWithoutNotify(false);
                    });
                });

                // Setup play button
                betterTogglePlay.OnValueChanged.AddListener((value) =>
                {
                    if (!ghostObject)
                        return;

                    ghostObject.Play(value,()=> 
                    {
                        betterTogglePlay.SetWithoutNotify(false);
                    });
                });


                // Show hide object
                button.gameObject.SetActive(true);
                button.onClick.AddListener(() =>
                {
                    if (ghostObject != null && ghostObject.datRecording == datRecording && ghostObject.gameObject.activeSelf)
                    {
                        ghostObject.gameObject.SetActive(false);
                        betterToggleRecord.gameObject.SetActive(false);
                        betterTogglePlay.gameObject.SetActive(false);
                    }
                    else
                    {
                        betterToggleRecord.gameObject.SetActive(true);
                        betterTogglePlay.gameObject.SetActive(true);
                        ghostObject = VisManager.instance.DemandGhostObject(datRecording);
                    }
                });
                break;
            case DatColor datColor:
                if (colorPicker)
                {
                    colorPicker.gameObject.SetActive(true);
                    colorPicker.SetColor(datColor.Value);
                    colorPicker.OnValueChanged.AddListener((value) =>
                    {
                        datColor.Value = value;
                    });
                    datColor.OnDataChanged += (value) =>
                    {
                        colorPicker.SetColor(datColor.Value);
                    };
                }
                break;
            default:
                break;
        }
    }

    public void OnDataChanged(VRData vrData)
    {
        if(textName)
            textName.text = vrVariable.Name();
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
