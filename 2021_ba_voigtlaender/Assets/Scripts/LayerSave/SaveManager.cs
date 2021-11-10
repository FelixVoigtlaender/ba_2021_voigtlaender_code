using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    int indexId = 0;

    public VRProgramm programm;
    //[TextArea(10,100)]
    string jsonText;

    public VRProgramm programmB;
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
    
    public void Update()
    {
        foreach (var element in saveElements)
        {
            element.Update();
        }
    }
    public void FixedUpdate()
    {
        foreach (var element in saveElements)
        {
            element.FixedUpdate();
        }
    }
}
