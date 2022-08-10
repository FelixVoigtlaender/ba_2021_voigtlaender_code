using System.Collections.Generic;
using DG.Tweening;
using LayerBottom;
using UnityEngine;
using UnityEngine.UI;

namespace LayerTop
{
    public class VisVariable : VisLogicElement
    {
        VRVariable vrVariable;
        public Slider slider;
        public Button button;
        public BetterToggle toggle;
        public Dropdown dropdownInt;
        public Dropdown dropdownFloat;
        List<SelectObject> selectObjects;



        VisVector visVector;
        VisObject visObject;


        public BetterToggle betterTogglePlay;
        public BetterToggle betterToggleRecord;

        public BetterColorPicker colorPicker;

        private void Start()
        {
            selectObjects = new List<SelectObject>(FindObjectsOfType<SelectObject>());

        }

        public override void Setup(VRLogicElement element)
        {
            visObject = GetComponentInParent<VisObject>();
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
                    dropdownInt.gameObject.SetActive(true);
                    dropdownFloat.gameObject.SetActive(true);

                    dropdownInt.ClearOptions();
                    List<Dropdown.OptionData> optionsInt = new List<Dropdown.OptionData>();
                    for (int i = 0; i < 10; i++)
                        optionsInt.Add(new Dropdown.OptionData("" + i));
                    dropdownInt.AddOptions(optionsInt);


                    dropdownFloat.ClearOptions();
                    List<Dropdown.OptionData> optionsFloat = new List<Dropdown.OptionData>();
                    for (int i = 0; i < 10; i++)
                        optionsFloat.Add(new Dropdown.OptionData("." + i));
                    dropdownFloat.AddOptions(optionsFloat);


                    int wholeNumber = Mathf.FloorToInt(datFloat.Value);
                    wholeNumber = Mathf.Clamp(wholeNumber, 0, 9);
                    int pointNumber = Mathf.RoundToInt((datFloat.Value - wholeNumber) * 10f);
                    pointNumber = Mathf.Clamp(pointNumber, 0, 9);
                    dropdownInt.value = wholeNumber;
                    dropdownFloat.value = pointNumber;

                    dropdownFloat.onValueChanged.RemoveAllListeners();
                    dropdownInt.onValueChanged.RemoveAllListeners();

                    dropdownFloat.onValueChanged.AddListener((value) => 
                    {
                        datFloat.Value = dropdownInt.value + dropdownFloat.value / 10f;
                    });
                    dropdownInt.onValueChanged.AddListener((value) => 
                    {
                        datFloat.Value = dropdownInt.value + dropdownFloat.value / 10f;
                    });
                    datFloat.OnDataChanged += (value) =>
                    {
                        wholeNumber = Mathf.FloorToInt(datFloat.Value);
                        wholeNumber = Mathf.Clamp(wholeNumber, 0, 9);
                        dropdownInt.SetValueWithoutNotify(wholeNumber);

                        pointNumber = Mathf.RoundToInt((datFloat.Value - wholeNumber) * 10f);
                        pointNumber = Mathf.Clamp(pointNumber, 0, 9);
                        dropdownFloat.SetValueWithoutNotify(pointNumber);
                    };

                    break;
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

                        GhostObject ghostObjectTransform = null;
                        if (!visObject)
                        {
                            ghostObjectTransform = VisManager.instance.DemandGhostObject(datTransform);
                            element.OnDelete += () =>
                            {
                                Destroy(ghostObjectTransform.gameObject);
                            };
                        }
                        else
                        {
                            ghostObjectTransform = visObject.ghostObject;
                        }


                        button.onClick.AddListener(() =>
                        {
                            ghostObjectTransform.Setup(datTransform);
                            ghostObjectTransform.gameObject.SetActive(true);
                            ghostObjectTransform.transform.position = transform.position;
                            ghostObjectTransform.transform.DOMove(datTransform.datPosition.Value,0.3f);
                        });
                    }
                    break;
                case DatBool datBool:
                    if (toggle)
                    {
                        toggle.gameObject.SetActive(true);
                        toggle.isOn = datBool.Value;
                        toggle.OnValueChanged.AddListener((value) =>
                        {
                            datBool.Value = value; textName.text = vrVariable.Name();
                        });
                    }
                    break;
                case DatRecording datRecording:
                    if (!button || !betterTogglePlay || !betterToggleRecord)
                        return;


                    GhostObject ghostObjectRec = null;
                    if (!visObject)
                    {
                        ghostObjectRec = VisManager.instance.DemandGhostObject(datRecording);
                        element.OnDelete += () =>
                        {
                            Destroy(ghostObjectRec.gameObject);
                        };
                    }
                    else
                    {
                        ghostObjectRec = visObject.ghostObject;
                    }


                    // Setup recording button
                    betterToggleRecord.OnValueChanged.AddListener((value) =>
                    {
                        if (!ghostObjectRec)
                            return;
                        ghostObjectRec.Record(value,()=> 
                        {
                            betterToggleRecord.SetWithoutNotify(false);
                        });
                    });

                    // Setup play button
                    betterTogglePlay.OnValueChanged.AddListener((value) =>
                    {
                        if (!ghostObjectRec)
                            return;

                        ghostObjectRec.Play(value,()=> 
                        {
                            betterTogglePlay.SetWithoutNotify(false);
                        });
                    });


                    // Show hide object
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(() =>
                    {
                        if (ghostObjectRec != null && ghostObjectRec.datRecording == datRecording && ghostObjectRec.gameObject.activeSelf)
                        {
                            ghostObjectRec.gameObject.SetActive(false);
                            betterToggleRecord.gameObject.SetActive(false);
                            betterTogglePlay.gameObject.SetActive(false);
                        }
                        else
                        {
                            betterToggleRecord.gameObject.SetActive(true);
                            betterTogglePlay.gameObject.SetActive(true);


                            ghostObjectRec.Setup(datRecording);
                            ghostObjectRec.gameObject.SetActive(true);
                            ghostObjectRec.transform.position = transform.position;
                            ghostObjectRec.transform.DOMove(datRecording.datTransform.datPosition.Value, 0.3f);
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
                case DatObj datObj:
                    if (!button)
                        return;
                    List<fvInputManager> inputManagers = new List<fvInputManager>(FindObjectsOfType<fvInputManager>());

                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(() => 
                    {
                        foreach(fvInputManager inputManager in inputManagers)
                        {
                            if (inputManager.uiRaycastHit.HasValue && inputManager.uiRaycastHit.Value.gameObject == button.gameObject)
                            {
                                if (textName)
                                    textName.text = "Select Object";
                                fvInputModeManager.instance.SwitchMode("SELECT");
                                fvInputModeManager.instance.OnModeChanged += OnModeChanged;
                                foreach (SelectObject selectObject in selectObjects)
                                {
                                    selectObject.OnSelectedObject += OnObjectSelected;
                                }


                                return;
                            }
                        }
                    });

                    break;
                default:
                    break;
            }
        }
        public void OnModeChanged(fvInputModeManager.Mode mode)
        {
            if (mode != null && mode.name == "SELECT")
                return;
            OnObjectSelected(null);
        }
         
        public void OnObjectSelected(GameObject go)
        {

            fvInputModeManager.instance.OnModeChanged -= OnModeChanged;
            fvInputModeManager.instance.SwitchMode("EDIT");
            foreach (SelectObject selectObject in selectObjects)
            {
                selectObject.OnSelectedObject -= OnObjectSelected;
            }
            if (!go)
            {
                if (textName)
                    textName.text = vrVariable.Name();
                return;
            }
        
        
            VRObject vrObject = VRManager.instance.InitVRObject(go,go.transform.position,false);
        

            DatObj datObj = new DatObj(vrObject);
        
            vrVariable.vrData.SetData(datObj);
            print(vrVariable.Name());
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
}
