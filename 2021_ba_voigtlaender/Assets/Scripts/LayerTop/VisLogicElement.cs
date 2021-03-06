using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VisLogicElement : MonoBehaviour
{
    protected VRLogicElement element;
    public Button myButton;
    public RectTransform inputHolder;
    public RectTransform outputHolder;
    public RectTransform variableHolder;
    public RectTransform tabHolder;
    public RectTransform tabToggleHolder;
    public Dropdown tabDropdown;
    public Text textName;
    Canvas rootCanvas;
    RectTransform rect;

    public List<VisPort> visInPorts;
    public List<VisPort> visOutPorts;
    public List<VisVariable> visVariables;
    public List<VisTab> visTab;

    public bool isDeleteAble = true;

    public virtual void Setup(VRLogicElement element)
    {
        this.element = element;
        textName.text = element.Name();
        element.OnDelete += OnDelete;
        PopulateVisPorts(element);

        visVariables = PopulateVisVariables(element, variableHolder);
        visTab = PopulateVisTabs(element, tabHolder);
        rootCanvas = GetRootCanvas();
        rect = GetComponent<RectTransform>();
    }

    public virtual void Init()
    {
    }

    public Canvas GetRootCanvas()
    {
        if (rootCanvas)
            return rootCanvas;

        rootCanvas = GetComponentInParent<Canvas>();
        return rootCanvas;
    }
    List<VisTab> PopulateVisTabs(VRLogicElement element, Transform holder)
    {
        if (!holder || element == null)
            return new List<VisTab>();

        if (element.vrTabs.Count == 0)
        {
            if (tabToggleHolder)
                tabToggleHolder.gameObject.SetActive(false);
            return new List<VisTab>();
        }

        if (!tabDropdown)
            return new List<VisTab>();


        tabDropdown.gameObject.SetActive(true);
        List<VisTab> visTabs = new List<VisTab>();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        options.Add(new Dropdown.OptionData(element.Name()));

        //VRDebug.Log("POPULATING VARIABLES " + element.vrVariables.Count);

        foreach (VRTab vrTab in element.vrTabs)
        {
            GameObject prefabVisTab = VisManager.instance.GetVisLogicPrefab(vrTab);
            if (!prefabVisTab)
            {

                VRDebug.Log("Couldn't find tab Prefab");
                continue;
            }

            // VisTab Setup
            GameObject objVisTab = Instantiate(prefabVisTab, holder);
            VisTab visTab = objVisTab.GetComponent<VisTab>();
            visTab.Setup(vrTab);
            visTab.SetOtherVisElement(this);
            // Add Dropdownoption
            Dropdown.OptionData option = new Dropdown.OptionData(vrTab.name);
            options.Add(option);

            visTabs.Add(visTab);
        }
        //Setup Dropdown
        tabDropdown.ClearOptions();
        tabDropdown.AddOptions(options);

        
        
        // Setup for visibility
        tabDropdown.onValueChanged.AddListener((value) => SetTabs(true));
        tabDropdown.onValueChanged.AddListener((value) => 
        {


            if (value == 0)
            {
                SetTabs(false);
            }
            else
            {

                SetTabs(true);
            }
            for (int i = 0; i < visTabs.Count; i++)
            {
                if (i == value - 1)
                {
                    visTabs[i].OnIsActiveChanged(true);
                }
                else
                {
                    visTabs[i].OnIsActiveChanged(false);
                }
            }
        });

        tabHolder.gameObject.SetActive(false);
        tabHolder.transform.localScale = new Vector3(0, 1, 1);


        return visTabs;
    }
    List<VisTab> PopulateVisTabsA(VRLogicElement element, Transform holder)
    {
        if (!holder || element == null)
            return new List<VisTab>();
        
        if (element.vrTabs.Count == 0)
        {
            if(tabToggleHolder)
                tabToggleHolder.gameObject.SetActive(false);
            return new List<VisTab>();
        }




        List<VisTab> visTabs = new List<VisTab>();
        GameObject prefabTabToggle = VisManager.instance.prefabTabToggle;
        //VRDebug.Log("POPULATING VARIABLES " + element.vrVariables.Count);
        ToggleGroup toggleGroup = tabToggleHolder.GetComponent<ToggleGroup>();


        foreach (VRTab vrTab in element.vrTabs)
        {
            GameObject prefabVisTab = VisManager.instance.GetVisLogicPrefab(vrTab);
            if (!prefabVisTab)
            {

                VRDebug.Log("Couldn't find tab Prefab");
                continue;
            }

            GameObject objTabToggle = Instantiate(prefabTabToggle, tabToggleHolder);
            GameObject objVisTab = Instantiate(prefabVisTab, holder);
            VisTab visTab = objVisTab.GetComponent<VisTab>();
            visTab.Setup(vrTab);

            Toggle toggle = objTabToggle.GetComponent<Toggle>();
            if (toggle.TryGetComponent(out TooltipContent tooltipContent))
                tooltipContent.description = vrTab.Name();
            toggle.onValueChanged.AddListener((value) => 
            {
                if (value)
                    SetTabs(true);
            });
            visTab.SetToggle(toggle);
            toggle.group = toggleGroup;

            visTabs.Add(visTab);
        }
        // Setup for visibility
        myButton.onClick.AddListener(ToggleTabs);

        tabHolder.gameObject.SetActive(false);
        tabHolder.transform.localScale = new Vector3(0, 1, 1);


        return visTabs;
    }

    public void ToggleTabs()
    {
        bool invValue = !tabHolder.gameObject.activeSelf;
        SetTabs(invValue);
    }

    public void SetTabs(bool value)
    {
        //tabToggleHolder.gameObject.SetActive(value);
        float easeTime = 0.1f;
        if (value)
        {
            tabHolder.gameObject.SetActive(value);
            tabHolder.transform.localScale = new Vector3(0, 1, 1);
            tabHolder.DOAnchorPos3DZ(-20, easeTime).OnComplete(() => tabHolder.DOScaleX(1, easeTime)); 
        }
        else
        {
            tabHolder.DOScaleX(0, easeTime).OnComplete(() => 
            {
                tabHolder.DOAnchorPos3DZ(0, easeTime);
            });
        }
    }

    List<VisVariable> PopulateVisVariables(VRLogicElement element, Transform holder)
    {
        if (!holder || element == null)
            return new List<VisVariable>();

        List<VisVariable> visVariables = new List<VisVariable>();

        //VRDebug.Log("POPULATING VARIABLES " + element.vrVariables.Count);

        foreach (VRVariable vrVariable in element.vrVariables)
        {
            GameObject prefabVisVariable = VisManager.instance.GetVisLogicPrefab(vrVariable);
            if (!prefabVisVariable)
            {

                VRDebug.Log("Couldn't find variable Prefab");
                continue;
            }

            GameObject objVisVariable = Instantiate(prefabVisVariable, holder);
            VisVariable visVariable = objVisVariable.GetComponent<VisVariable>();
            visVariable.Setup(vrVariable);
            visVariables.Add(visVariable);
        }
        return visVariables;
    }

    public void PopulateVisPorts(VRLogicElement element)
    {
        //Inputs
        visInPorts = PopulateVisPort(inputHolder, element.vrInputs);
        visOutPorts = PopulateVisPort(outputHolder, element.vrOutputs);
    }
    List<VisPort> PopulateVisPort(Transform holder, List<VRPort> ports)
    {
        GameObject prefabVisPort = VisManager.instance.prefabVisPort;
        List<VisPort> visPorts = new List<VisPort>();
        foreach (VRPort vrPort in ports)
        {
            GameObject objVisPort = Instantiate(prefabVisPort, holder);
            VisPort visPort = objVisPort.GetComponent<VisPort>();
            visPort.Setup(vrPort);
            visPorts.Add(visPort);
        }
        return visPorts;
    }

    public virtual bool IsType(VRLogicElement vrLogicElement) 
    {
        return true;
    }

    public virtual void OnDelete()
    {
        VRDebug.Log("VisDeleting: " + gameObject.name);
        if (rootCanvas.gameObject)
            Destroy(rootCanvas.gameObject);
    }
    public void Delete()
    {
        if (!isDeleteAble)
            return;

        VisLogicElement[] parentVisElements = GetComponentsInParent<VisLogicElement>();
        if (parentVisElements.Length == 1)
        {
            element.Delete();
            return;
        }

        foreach(VisLogicElement visElement in parentVisElements)
        {
            if(visElement!= this)
            {
                visElement.Delete();
                return;
            }

        }
    }

    public void Trigger()
    {
        element.Trigger();
    }
}
