using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisLogicElement : MonoBehaviour
{
    VRLogicElement element;
    public Transform inputHolder;
    public Transform outputHolder;
    public Text textName;
    Canvas rootCanvas;

    public List<VisPort> visInPorts;
    public List<VisPort> visOutPorts;

    public virtual void Setup(VRLogicElement element)
    {
        this.element = element;
        textName.text = element.Name();
        element.OnDelete += OnDelete;
        PopulateVisPorts(element);

        rootCanvas = GetComponentInParent<Canvas>();
    }

    public virtual void Init()
    {
    }

    public void PopulateVisPorts(VRLogicElement element)
    {
        //Inputs
        visInPorts = PopulateVisPort(inputHolder, element.vrInputs);
        visOutPorts = PopulateVisPort(outputHolder, element.vrOutputs);
    }
    List<VisPort> PopulateVisPort(Transform parent, List<VRPort> ports)
    {
        GameObject prefabVisPort = VisManager.instance.prefabVisPort;
        List<VisPort> visPorts = new List<VisPort>();
        foreach (VRPort vrPort in ports)
        {
            GameObject objVisPort = Instantiate(prefabVisPort, parent);
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

    public void OnDelete()
    {
        if (rootCanvas.gameObject)
            Destroy(rootCanvas.gameObject);
    }
    public void Delete()
    {
        element.Delete();
    }
}
