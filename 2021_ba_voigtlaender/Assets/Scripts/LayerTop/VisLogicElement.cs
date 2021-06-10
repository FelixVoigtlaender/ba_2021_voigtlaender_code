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

    public List<VisPort> visInPorts;
    public List<VisPort> visOutPorts;

    public void Setup(VRLogicElement element)
    {
        this.element = element;
        textName.text = element.Name();
        PopulateVisPorts(element);
    }

    public void PopulateVisPorts(VRLogicElement element)
    {
        //Inputs
        visInPorts = PopulateVisPort(inputHolder, element.inputs);
        visOutPorts = PopulateVisPort(outputHolder, element.outputs);
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
}
