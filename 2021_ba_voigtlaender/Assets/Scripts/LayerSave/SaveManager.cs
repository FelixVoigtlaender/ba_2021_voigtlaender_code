using System.Collections;
using System.Collections.Generic;
using System.IO;
using LayerBottom;
using LayerTop;
using UnityEngine;

namespace LayerSave
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager instance;
        int indexId = 0;

        public VRProgramm programm;
        //[TextArea(10,100)]
        string jsonText;

        private List<GameObject> relevantGameObjects = new List<GameObject>();
    
        private void Awake()
        {
            instance = this;
            GenerateRelevantGameObjects();
            programm = CreateVRProgramm();
        }


        void GenerateRelevantGameObjects()
        {
            Collider[] colliders = FindObjectsOfType<Collider>();
            foreach (var collider in colliders)
            {
                relevantGameObjects.Add(collider.gameObject);
            }
        }
    
        public static int GenerateID()
        {
            if (!instance)
                return -1;
            instance.indexId++;
            return instance.indexId;
        }
        public static VRProgramm CreateVRProgramm()
        {
            VRProgramm programm = new VRProgramm();
            foreach (var go in instance.relevantGameObjects)
            {
                programm.gameObjectReferences.Add(new GameObjectReference(go));
            }
        
            return programm;
        }
        public static void AddSaveElement(SaveElement saveElement)
        {
            if (!instance)
                return;

            if (instance.programm == null)
                instance.programm = CreateVRProgramm();
        
            if(instance.programm.saveElements.Contains(saveElement))
                return;

            //print($"isRoot: {saveElement.isRoot} Saving: {saveElement.ToString()}");

            instance.programm.saveElements.Add(saveElement);

            instance.jsonText = ToJson(instance.programm);
        }
        public static void RemoveSaveElement(SaveElement saveElement)
        {
            if (!instance)
                return;

            if (instance.programm == null)
                instance.programm = CreateVRProgramm();

            instance.programm.saveElements.Remove(saveElement);
            instance.jsonText = ToJson(instance.programm);
        }

        public static string ToJson(VRProgramm vrProgramm)
        {
            return JsonUtility.ToJson(vrProgramm,true);
        }
        public static VRProgramm ToProgramm(string jsonText)
        {
            return JsonUtility.FromJson<VRProgramm>(jsonText);
        }


        [ContextMenu("Destroy VisProgram")]
        public void DestroyVisProgram()
        {
            VisManager.instance.DestroyVisProgram();

        }


        public void Save()
        {
            if(programm == null)
                return;
        
            string jsonString = ToJson(programm);
            string path = Application.persistentDataPath;
            string filePath = path + "/program.json";
            File.WriteAllText(filePath, jsonString);
        
            Debug.Log($"Saved program to {filePath} \n {jsonString}");
        }

        public void Delete()
        {
            string path = Application.persistentDataPath;
            string filePath = path + "/program.json";

            if (System.IO.File.Exists(filePath))
                File.Delete(filePath);
        
        
            Debug.Log($"Deleted program at {filePath}");
        }

        public void Load()
        {
            DestroyVisProgram();
        
        
            string path = Application.persistentDataPath;
            string filePath = path + "/program.json";

            if (!System.IO.File.Exists(filePath))
            {
                Debug.Log($"Couldn't Load {filePath}");
                return;
            }

            string jsonString = File.ReadAllText(filePath);
            programm = ToProgramm(jsonString);
            programm.LoadObjects();
        
            bool success = VisManager.instance.VisProgramm(programm);

            if (!success)
            {
                Debug.Log($"Loaded Program {filePath} \n {jsonString}");
            }
            else
            {
                Delete();
                Debug.Log($"Something went wrong while visualising the program {filePath} \n {jsonString}");
            }
        }
    }

    [System.Serializable]
    public class VRProgramm
    {
        [SerializeReference]
        public List<SaveElement> saveElements = new List<SaveElement>();

        [SerializeReference] public List<GameObjectReference> gameObjectReferences = new List<GameObjectReference>();


        public void Start(DatEvent datEvent)
        {
            foreach (var element in saveElements)
            {
                element.Start(datEvent);
            }
        }
        public void Update(DatEvent datEvent)
        {
            foreach (var element in saveElements)
            {
                element.Update(datEvent);
            }
        }
        public void FixedUpdate(DatEvent datEvent)
        {
            foreach (var element in saveElements)
            {
                element.FixedUpdate(datEvent);
            }
        }


        public void SaveObjects()
        {
            foreach (var go in gameObjectReferences)
            {
                go.Save();
            }
        }

        public void LoadObjects()
        {
            foreach (var go in gameObjectReferences)
            {
                go.Load();
            }
        }
    }

    [System.Serializable]
    public class GameObjectReference
    {
    
        [SerializeReference] public GameObject gameObject;
        public Vector3 position;
        public Vector3 localScale;
        public Quaternion rotation;
        public bool isActive;

        public GameObjectReference(GameObject gameObject)
        {
            this.gameObject = gameObject;
            Save();
        }
    
        public void Save()
        {
            position = gameObject.transform.position;
            localScale = gameObject.transform.localScale;
            rotation = gameObject.transform.rotation;
            isActive = gameObject.activeSelf;
        }

        public void Load()
        {
            gameObject.transform.position = position;
            gameObject.transform.localScale = localScale;
            gameObject.transform.rotation = rotation;
            gameObject.SetActive(isActive);
        }
    }
}