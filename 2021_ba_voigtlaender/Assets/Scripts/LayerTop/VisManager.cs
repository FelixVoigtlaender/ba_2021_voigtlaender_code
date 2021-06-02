using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisManager : MonoBehaviour
{
    public static VisManager instance;
    public GameObject prefabVisPort;
    public GameObject[] prefabVisProperties;

    private void Awake()
    {
        instance = this;
    }


    public Color GetDataColor(VRData vrData)
    {
        return Color.black;
    }

    public GameObject GetVisPropertyPrefab(VRProperty vrPorperty)
    {
        foreach(GameObject prefab in prefabVisProperties)
        {
            VisProperty visProperty = prefab.GetComponent<VisProperty>();
            return prefab;
        }
        return null;
    }
}


