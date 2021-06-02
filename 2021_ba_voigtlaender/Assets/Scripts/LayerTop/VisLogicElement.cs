using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisLogicElement : MonoBehaviour
{
    VRLogicElement element;
    public Transform inputHolder;
    public Transform outputHolder;

    public void Setup(VRLogicElement element)
    {
        this.element = element;
        PopulateVisPorts(element);
    }

    public void PopulateVisPorts(VRLogicElement element)
    {
        //Inputs
        PopulateVisPort(inputHolder, element.inputs);
        PopulateVisPort(outputHolder, element.outputs);
    }
    List<VisPort> PopulateVisPort(Transform parent, List<VRPort> ports)
    {
        GameObject prefabVisPort = VisManager.instance.prefabVisPort;
        List<VisPort> visPorts = new List<VisPort>();
        foreach (VRPort vrPort in element.inputs)
        {
            GameObject objVisPort = Instantiate(prefabVisPort, parent);
            VisPort visPort = objVisPort.GetComponent<VisPort>();
            visPort.Setup(vrPort);
            visPorts.Add(visPort);
        }
        return visPorts;
    }

    public abstract bool IsType(VRLogicElement vrLogicElement);
}
