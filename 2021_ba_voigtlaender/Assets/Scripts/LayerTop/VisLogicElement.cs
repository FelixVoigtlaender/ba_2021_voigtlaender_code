using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisLogicElement : MonoBehaviour
{
    VRLogicElement element;
    public Transform inputHolder;
    public Transform outputHolder;
    public Transform variableHolder;
    public Text textName;
    Canvas rootCanvas;

    public List<VisPort> visInPorts;
    public List<VisPort> visOutPorts;
    public List<VisVariable> visVariables;

    public virtual void Setup(VRLogicElement element)
    {
        this.element = element;
        textName.text = element.Name();
        element.OnDelete += OnDelete;
        PopulateVisPorts(element);

        visVariables = PopulateVisVariables(element, variableHolder);

        rootCanvas = GetComponentInParent<Canvas>();
    }

    public virtual void Init()
    {
    }


    List<VisVariable> PopulateVisVariables(VRLogicElement element, Transform holder)
    {
        if (!holder || element == null)
            return new List<VisVariable>();

        List<VisVariable> visVariables = new List<VisVariable>();

        VRDebug.Log("POPULATING VARIABLES " + element.vrVariables.Count);

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
        if (rootCanvas.gameObject)
            Destroy(rootCanvas.gameObject);
    }
    public void Delete()
    {
        element.Delete();
    }
}
