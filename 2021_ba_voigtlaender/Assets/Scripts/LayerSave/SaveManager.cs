using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro.EditorUtilities;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    int indexId = 0;

    public VRProgramm programm;
    //[TextArea(10,100)]
    string jsonText;
    private void Awake()
    {
        instance = this;
        programm = CreateVRProgramm();
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
        return new VRProgramm();
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
        VisManager.instance.VisProgramm(programm);
        
        Debug.Log($"Loaded Program {filePath} \n {jsonString}");
    }
    
    
    [ContextMenu("Load Program")]
    public void LoadProgram()
    {
        if (programm == null)
            return;
        VRProgramm oldProgram = programm;
        programm = CreateVRProgramm();
        VisManager.instance.VisProgramm(oldProgram);
    }
}

[System.Serializable]
public class VRProgramm
{
    [SerializeReference]
    public List<SaveElement> saveElements = new List<SaveElement>();
    
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
}
