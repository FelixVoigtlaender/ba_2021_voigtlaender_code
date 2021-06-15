using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisObject : MonoBehaviour
{
    VRObject vrObject;
    List<VisProperty> visProperties = new List<VisProperty>();

    public Transform propertyHolder;
    public Text textName;
    public BezierCurve lineToObject;
    public float hoverDistance = 2;

    public Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void Setup(VRObject vrObject)
    {
        canvas.transform.position = vrObject.gameObject.transform.position + Vector3.up*hoverDistance;

        this.vrObject = vrObject;
        textName.text = vrObject.gameObject.name;
        vrObject.OnDelete += OnDelete;

        lineToObject.start.Connect(vrObject.gameObject.transform);
        lineToObject.end.Connect(canvas.transform);

        PopulateProperties();
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
    }
}
