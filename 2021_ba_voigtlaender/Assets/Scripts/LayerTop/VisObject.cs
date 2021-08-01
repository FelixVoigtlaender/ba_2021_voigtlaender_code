using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisObject : MonoBehaviour
{
    VRObject vrObject;
    List<VisProperty> visProperties = new List<VisProperty>();
    public List<VisPort> visInPorts;
    public List<VisPort> visOutPorts;

    public Transform propertyHolder;
    public RectTransform inputHolder;
    public RectTransform outputHolder;
    public RectTransform secondaryInputHolder;
    public RectTransform secondaryOutputHolder;

    public Text textName;
    public BezierCurve lineToObject;
    public float hoverDistance = 2;
    public MiniatureMaker miniature;
    public GhostObject ghostObject;

    public Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void Setup(VRObject vrObject)
    {
        ghostObject = VisManager.instance.DemandGhostObject(new DatTransform(new DatObj(vrObject)));

        canvas.transform.position = vrObject.gameObject.transform.position + Vector3.up*hoverDistance;

        this.vrObject = vrObject;
        textName.text ="Object: "+ vrObject.gameObject.name;
        vrObject.OnDelete += OnDelete;

        miniature.CopyMesh(vrObject.gameObject);

        lineToObject.start.Connect(vrObject.gameObject.transform);
        lineToObject.start.useLocalSpace = false;
        lineToObject.start.dynamicNormals = false;
        lineToObject.start.normal = Vector3.up;

        lineToObject.end.Connect(canvas.transform);
        lineToObject.end.useLocalSpace = false;
        lineToObject.end.dynamicNormals = false;
        lineToObject.end.normal = Vector3.down;

        PopulateProperties();
        PopulateVisPorts(vrObject.vrInputs,vrObject.vrOutputs);


    }

    public void PopulateProperties()
    {
        foreach(VRProperty vrProperty in vrObject.properties)
        {
            GameObject propertyPrefab = VisManager.instance.GetVisPropertyPrefab(vrProperty);
            if (!propertyPrefab)
                continue;

            GameObject propertyObj = Instantiate(propertyPrefab, propertyHolder);
            VisProperty visProperty = propertyObj.GetComponent<VisProperty>();
            visProperty.Setup(vrProperty);
            visProperties.Add(visProperty);
        }
    }

    public void PopulateVisPorts(List<VRPort> inputs, List<VRPort> outputs)
    {
        //Inputs
        visInPorts = PopulateVisPort(inputHolder, inputs);
        visOutPorts = PopulateVisPort(outputHolder, outputs);


        visInPorts.AddRange(PopulateVisPort(secondaryInputHolder, inputs));
        visOutPorts.AddRange(PopulateVisPort(secondaryOutputHolder, outputs));
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
    public void Delete()
    {
        vrObject.Delete();
    }
    public void OnDelete()
    {
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if(rootCanvas && rootCanvas.gameObject)
        {
            Destroy(rootCanvas.gameObject);
        }

        Destroy(ghostObject);
    }
}
