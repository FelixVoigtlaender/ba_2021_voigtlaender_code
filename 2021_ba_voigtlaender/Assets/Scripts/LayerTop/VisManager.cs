using System;
using System.Collections;
using System.Collections.Generic;
using LayerBottom;
using LayerSave;
using UnityEngine;

namespace LayerTop
{
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
        [Header("Debug")]
        public GameObject prefabDebugSphere;
        public GameObject prefabDebugLine;

        [Header("Ghost")]
        private VisVector visVector;
        private Transform visVectorTrans;
        private GhostObject ghostObject;

        public Transform programParent;


        private List<OutlineObject> outlineObjects = new List<OutlineObject>();

        private void Awake()
        {
            instance = this;

            programParent = new GameObject("Program Parent").transform;
            programParent.parent = gameObject.transform;
            programParent.localPosition = Vector3.zero;
            programParent.localRotation = Quaternion.identity;
            programParent.localScale = Vector3.one;
            
            
        
        }

            
        
        public void HandleOutlines()
        {
            Collider[] colliders = FindObjectsOfType<Collider>();
            foreach (var collider in colliders)
            {
                if (collider.gameObject.GetComponentInChildren<Renderer>())
                {
                    OutlineObject outlineObject = new OutlineObject(collider.gameObject);
                    outlineObjects.Add(outlineObject);
                }
                
            }
        }

        public OutlineObject FindOutlineObject(GameObject gameObject)
        {
            foreach (var outlineObject in outlineObjects)
            {
                if (outlineObject.gameObject == gameObject)
                    return outlineObject;
            }

            return null;
        }


        public void SetVisibility(bool value)
        {
            programParent.gameObject.SetActive(value);
        }

        private void Start()
        {
            HandleOutlines();
            VRManager.instance.OnInitVRObject += OnInitVRObjectNORETURN;
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

        public GhostObject DemandGhostObject(DatTransform datTransform)
        {
            if (datTransform == null ||  datTransform.datObj == null)
                return null;
        

            GameObject ghostObj = Instantiate(prefabGhostObject);
            ghostObject = ghostObj.GetComponent<GhostObject>();

            ghostObject.Setup(datTransform);
            ghostObject.gameObject.SetActive(true);

            return ghostObject;
        }


        public GhostObject DemandGhostObject(DatRecording datRecording)
        {
            if (datRecording == null)
                return null;

            if (!ghostObject)
            {
                GameObject ghostObj = Instantiate(prefabGhostObject);
                ghostObject = ghostObj.GetComponent<GhostObject>();
            }

            ghostObject.Setup(datRecording);
            ghostObject.gameObject.SetActive(true);

            return ghostObject;
        }
        public void OnInitVRObjectNORETURN(VRObject vrObject)
        {
            OnInitVRObject(vrObject);
        }
        public VisObject OnInitVRObject(VRObject vrObject)
        {
            GameObject visObjectObj = Instantiate(prefabVisObject);
            visObjectObj.transform.parent = programParent;
            VisObject visObject = visObjectObj.GetComponent<VisObject>();
            visObject.Setup(vrObject);

            return visObject;
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
            GameObject obj = null;
            if (!prefab.TryGetComponent(out Canvas canvas))
            {
                GameObject objCanvas = Instantiate(prefabUICanvas);
                objCanvas.transform.position = position;
                objCanvas.transform.parent = programParent;

                PanelHolder panelHolder = objCanvas.GetComponentInChildren<PanelHolder>();

                if (panelHolder)
                    obj = Instantiate(prefab, panelHolder.transform);
                else
                    obj = Instantiate(prefab, objCanvas.transform);
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
            foreach (GameObject prefab in prefabVisProperties)
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

        public bool VisProgramm(VRProgramm vrProgramm)
        {
            try
            {
                foreach (SaveElement saveElement in vrProgramm.saveElements)
                {
                    print(saveElement.ToString() + " " + saveElement.isRoot);
                    if (!saveElement.isRoot)
                        continue;

                    if (saveElement is VRObject)
                    {
                        VRObject vrObject = (VRObject)saveElement;
                        print(vrObject.gameObject.name);
                        VisObject visObject = OnInitVRObject(vrObject);
                        visObject.transform.position = saveElement.position;
                        continue;
                    }
                    if(saveElement is VRLogicElement)
                    {
                        VRLogicElement vrLogicElement = (VRLogicElement)saveElement;
                        InstantiateElement(vrLogicElement, saveElement.position);
                        continue;
                    }
                    if(saveElement is VRConnection)
                    {
                        VRConnection vrConnection = (VRConnection)saveElement;

                        Debug.Log(JsonUtility.ToJson(vrConnection, true));
                
                
                        GameObject objVisConnection = Instantiate(prefabVisConnection);
                        objVisConnection.GetComponent<VisConnection>().Setup(vrConnection);
                        continue;
                    }

                    Debug.LogError($"Couldn't load {saveElement.ToString()}");
                }

            }
            catch (Exception e)
            {
                Debug.Log("Coudn't visualize the program!");
                DestroyVisProgram();
                return false;
            }

            return true;
        }
        public VisLogicElement InstantiateElement(VRLogicElement logicElement, Vector3 position)
        {
            GameObject elementPrefab = VisManager.instance.GetVisLogicPrefab(logicElement);
            GameObject objLogicElement = VisManager.instance.InitPrefabWithCanvas(elementPrefab, position);
            VisLogicElement visElement = objLogicElement.GetComponent<VisLogicElement>();
            visElement.Setup(logicElement);
            return visElement;
        } 


        [ContextMenu("Destroy Program")]
        public void DestroyVisProgram()
        {
            VisLogicElement[] visLogicElements = FindObjectsOfType<VisLogicElement>();
            foreach (VisLogicElement visElement in visLogicElements)
            {
                if (visElement.GetElement().isRoot && visElement.isDeleteAble)
                    Destroy(visElement.GetRootCanvas().gameObject);
            }
            VisObject[] visObjects = FindObjectsOfType<VisObject>();
            foreach (VisObject visObject in visObjects)
            {
                Destroy(visObject.gameObject);
            }
            VisConnection[] visConnections = FindObjectsOfType<VisConnection>();
            foreach (var item in visConnections)
            {
                Destroy(item.gameObject);
            }
        }
    }


    public class VisVector
    {
        public Transform transform;
    }

    public class OutlineObject
    {
        public GameObject gameObject;
        public QuickOutline quickOutline;
        public OutlineObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
            quickOutline = gameObject.AddComponent<QuickOutline>();
            quickOutline.enabled = false;
        }
    }
}