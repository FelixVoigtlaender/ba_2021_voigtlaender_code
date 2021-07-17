using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisManager : MonoBehaviour
{
    public static VisManager instance;

    [Header("Canvas")]
    public GameObject prefabUICanvas; //TODO

    public GameObject prefabVisConnection;

    [Header("Prefabs")]
    public GameObject prefabVisObject;
    public GameObject prefabVisVector;
    public GameObject prefabTabToggle;
    public GameObject prefabGhostObject;
    [Header("Property")]
    public GameObject[] prefabVisProperties;
    public GameObject[] prefabVisEvents;
    public GameObject[] prefabLogicElements;
    public GameObject prefabVisPort;

    [Header("Ghost")]
    private VisVector visVector;
    private Transform visVectorTrans;

    private GhostObject ghostObject;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        VRManager.instance.OnInitVRObject += OnInitVRObject;
        VRManager.instance.OnInitVREvent += OnInitVREvent;
        VRManager.instance.OnInitVRVariable += OnInitVRVariable;
    }

    public VisVector DemandVisVector()
    {
        if (!visVectorTrans)
            visVectorTrans = Instantiate(prefabVisVector).transform;
        if (visVector != null)
            visVector.transform = null;

        visVector = new VisVector();
        visVector.transform = visVectorTrans;
        visVector.transform.gameObject.SetActive(true);

        return visVector;
    }

    public GhostObject DemandGhostObject( DatTransform datTransform)
    {
        if ( datTransform == null)
            return null;

        if (!ghostObject)
        {
            GameObject ghostObj = Instantiate(prefabGhostObject);
            ghostObject = ghostObj.GetComponent<GhostObject>();
        }

        ghostObject.Setup(datTransform);
        ghostObject.gameObject.SetActive(true);

        return ghostObject;
    }

    public void OnInitVRObject(VRObject vrObject)
    {
        GameObject visObjectObj = Instantiate(prefabVisObject);
        VisObject visObject = visObjectObj.GetComponent<VisObject>();
        visObject.Setup(vrObject);
    }
    // TODO:
    public void OnInitVREvent(VREvent vrEvent)
    {
        GameObject visElemetPrefab = GetVisLogicPrefab(vrEvent);
        if (!visElemetPrefab)
        {
            VRDebug.Log("Couldn't find visElemetPrefab!");
            return;
        }

        GameObject visElementObj = Instantiate(visElemetPrefab);
        // Position element infront of camera
        Vector3 position = Camera.main.transform.forward * 1 + Camera.main.transform.position;
        visElementObj.transform.position = position;
        // Setip event
        VisEvent visEvent = visElementObj.GetComponent<VisEvent>(); 
        visEvent.Setup(vrEvent);
    }

    public void OnInitVRVariable(VRVariable vrVariable)
    {
        GameObject visElemetPrefab = GetVisLogicPrefab(vrVariable);
        if (!visElemetPrefab)
        {
            VRDebug.Log("Couldn't find visElemetPrefab!");
            return;
        }

        GameObject visElementObj = Instantiate(visElemetPrefab);
        // Position element infront of camera
        Vector3 position = Camera.main.transform.forward * 1 + Camera.main.transform.position;
        visElementObj.transform.position = position;
        // Setup Variable
        VisVariable visVariable = visElementObj.GetComponent<VisVariable>();
        visVariable.Setup(vrVariable);
    }

    public GameObject InitVRLogicElement(VRLogicElement vrLogicElement)
    {
        GameObject prefab = GetVisLogicPrefab(vrLogicElement);
        if (!prefab)
            return null;

        return InitPrefab(prefab);
    }
    public GameObject InitPrefab(GameObject prefab)
    {
        Vector3 position = Camera.main.transform.forward * 1 + Camera.main.transform.position;
        return InitPrefabWithCanvas(prefab, position);
    }
    public GameObject InitPrefabWithCanvas(GameObject prefab, Vector3 position)
    {
        GameObject obj =null;
        if (!prefab.TryGetComponent(out Canvas canvas))
        {
            GameObject objCanvas = Instantiate(prefabUICanvas);
            objCanvas.transform.position = position;

            PanelHolder panelHolder = objCanvas.GetComponentInChildren<PanelHolder>();

            if(panelHolder)
                obj = Instantiate(prefab, panelHolder.transform);
            else
                obj = Instantiate(prefab,objCanvas.transform);
        }
        else
        {
            obj = Instantiate(prefab);
            obj.transform.position = position;
        }
        return obj;
    }


    public GameObject GetVisPropertyPrefab(VRProperty vrPorperty)
    {
        foreach(GameObject prefab in prefabVisProperties)
        {
            VisProperty visProperty = prefab.GetComponent<VisProperty>();

            if (!visProperty.IsType(vrPorperty))
                continue;

            return prefab;
        }
        return null;
    }

    public GameObject GetVisLogicPrefab(VRLogicElement vrLogicElement)
    {
        foreach (GameObject prefab in prefabLogicElements)
        {
            VisLogicElement visLogicElement = prefab.GetComponent<VisLogicElement>();

            if (!visLogicElement.IsType(vrLogicElement))
                continue;

            return prefab;
        }
        return null;
    }
}


public class VisVector
{
    public Transform transform;
}

