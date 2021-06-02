using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisObject : MonoBehaviour
{
    VRObject vrObject;
    List<VisProperty> visProperties = new List<VisProperty>();

    private void Start()
    {
        vrObject = GetComponent<VRObject>();
    }

    public void Setup(VRObject vrObject)
    {
        this.vrObject = vrObject;


    }

    public void PopulateProperties()
    {
        foreach(VRProperty vrProperty in vrObject.properties)
        {
            GameObject propertyPrefab = VisManager.instance.GetVisPropertyPrefab(vrProperty);
            if (!propertyPrefab)
                continue;

            GameObject propertyObj = Instantiate(propertyPrefab, transform);
            VisProperty visProperty = propertyObj.GetComponent<VisProperty>();
            visProperty.Setup(vrProperty);
            visProperties.Add(visProperty);
        }
    }
}
