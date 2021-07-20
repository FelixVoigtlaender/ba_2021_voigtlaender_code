using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public abstract class VRProperty : VRLogicElement
{
    protected VRObject vrObject;
    protected VRPort output;
    protected VRPort input;

    static List<VRProperty> allProperties;

    public abstract VRData GetData();
    
    public abstract bool IsType(VRObject vrObject);
    public virtual void Setup(VRObject vrObject) 
    {
        this.vrObject = vrObject;
        base.Setup();
    }


    public virtual bool CanBeUsed()
    {
        return true;
    }
    public static List<VRProperty> GetAllPorperties()
    {
        if (allProperties != null)
            return allProperties;


        allProperties = new List<VRProperty>();
        IEnumerable<Type> subClasses = VRManager.GetAllSubclassOf(typeof(VRProperty));
        foreach(Type type in subClasses)
        {
            VRProperty vrProperty = (VRProperty) Activator.CreateInstance(type);
            if(vrProperty.CanBeUsed())
                allProperties.Add(vrProperty);
        }
        return allProperties;
    }

}


public class PropTrigger : VRProperty
{
    VRVariable eventVariable;
    public override string Name()
    {
        return "Apply Changes";
    }
    public override bool IsType(VRObject vrObject)
    {
        // Gameobject always can be triggered
        return true;
    }

    public override void SetupVariables()
    {
        base.SetupVariables();
        eventVariable = new VRVariable();
        eventVariable.Setup(new DatEvent(-1));
        eventVariable.name = "Apply Changes";
        eventVariable.OnSetData += SetData;
        //eventVariable.OnVariableChanged+= SetData;

        vrVariables.Add(eventVariable);
    }


    public override void SetupOutputs()
    {
        base.SetupOutputs();
        output = new VRPort(GetData, new DatEvent(-1));
        //vrOutputs.Add(output);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        input = new VRPort(SetData, new DatEvent(-1));
        //vrInputs.Add(input);
    }

    public override VRData GetData()
    {
        return new DatEvent(-1);
    }
    public void SetData(VRData vrData)
    {
        VRDebug.Log("Applying to: " + vrObject.gameObject.name);
        vrObject.Trigger();
    }

    public override void Trigger()
    {
    }

}


public class PropEnabled : VRProperty
{
    VRVariable varEnabled;
    public override string Name()
    {
        return "Enabled";
    }
    public override bool IsType(VRObject vrObject)
    {
        // Gameobject always can be enabled
        return true;
    }

    public override void SetupVariables()
    {
        base.SetupVariables();
        varEnabled = new VRVariable();
        varEnabled.Setup(new DatBool(vrObject.gameObject.activeSelf));
        varEnabled.name = "Enabled";
        varEnabled.OnSetData += SetData;

        vrVariables.Add(varEnabled);
    }

    public void SetData(VRData vrData)
    {
        Trigger();
    }

    public override void Trigger()
    {
        DatBool datEnabled = (DatBool)varEnabled.GetData();
        vrObject.gameObject.SetActive(datEnabled.Value);
    }

    public override VRData GetData()
    {
        return varEnabled.GetData();
    }
}

public class PropObj : VRProperty
{
    public override string Name()
    {
        return "Object";
    }
    public override bool IsType(VRObject vrObject)
    {
        // Gameobject always has a transform
        return true;
    }

    public override void SetupOutputs()
    {
        base.SetupOutputs();
        output = new VRPort(GetData, new DatObj(new VRObject()));
        vrOutputs.Add(output);
    }
    public override void SetupInputs()
    {
        base.SetupInputs();
        input = new VRPort(this, new DatObj(new VRObject()));
        //vrInputs.Add(input);
    }

    public override VRData GetData()
    {
        VRDebug.print("GettingObject: " + vrObject.gameObject);
        return new DatObj(vrObject);
    }

    public override void Trigger()
    {
    }

}


public class PropPosition : VRProperty
{
    VRVariable positionVariable;

    public override bool CanBeUsed()
    {
        return false;
    }

    public override string Name()
    {
        return "Position";
    }
    public override bool IsType(VRObject vrObject)
    {
        // Only world objects should be moved
        RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
        RectTransform canvas = vrObject.gameObject.GetComponent<RectTransform>();

        if (canvas)
            return true;
        if (rect)
            return false;


        return true;
    }


    public override void SetupVariables()
    {
        base.SetupVariables();
        positionVariable = new VRVariable();
        positionVariable.Setup(new DatVector3(vrObject.gameObject.transform.position));
        positionVariable.name = "Position";
        positionVariable.OnSetData += SetData;
        positionVariable.OnGetData += GetData;

        vrVariables.Add(positionVariable);
    }

    public override VRData GetData()
    {
        return new DatVector3(vrObject.gameObject.transform.position);
    }

    public void SetData(VRData vrData)
    {
        DatVector3 datVector = (DatVector3)vrData;
        vrObject.gameObject.transform.position = datVector.Value;
    }

    public override void Trigger()
    {
        DatVector3 vrVector3 = (DatVector3)positionVariable.vrData;
        vrObject.gameObject.transform.position = vrVector3.Value;
    }

}


public class PropMovement : VRProperty
{
    // Teleport
    VRTab tabTeleport;
    VRTab tabMove;

    // Variables
    VRVariable varPosition;
    VRVariable varDuration;
    public override string Name()
    {
        return "Movement";
    }
    public override bool IsType(VRObject vrObject)
    {
        RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
        RectTransform canvas = vrObject.gameObject.GetComponent<RectTransform>();

        if (canvas)
            return true;
        if (rect)
            return false;


        return true;
    }

    public override bool CanBeUsed()
    {
        return false;
    }
    public override void SetupTabs()
    {
        base.SetupTabs();

        // Variables
        varPosition = new VRVariable(new DatVector3(vrObject.gameObject.transform.position), "Position");
        varDuration = new VRVariable(new DatFloat(1), "Duration");
        varDuration.allowDatName = true;

        // Tabs
        tabTeleport = new VRTab("Teleport");
        tabTeleport.vrVariables.Add(varPosition);
        vrTabs.Add(tabTeleport);

        tabMove = new VRTab("Move");
        tabMove.vrVariables.Add(varPosition);
        tabMove.vrVariables.Add(varDuration);
        vrTabs.Add(tabMove);
    }

    public override void Trigger()
    {
        VRTab activeTab = GetActiveTab();
        if (activeTab == null)
            return;

        if(activeTab == tabTeleport)
        {
            DatVector3 vrVector3 = (DatVector3)varPosition.vrData;
            vrObject.gameObject.transform.position = vrVector3.Value;
            return;
        }
        if (activeTab == tabMove)
        {
            DatVector3 datPosition = (DatVector3)varPosition.vrData;
            DatFloat datDuration = (DatFloat)varDuration.vrData;
            vrObject.gameObject.transform.DOMove(datPosition.Value, datDuration.Value);
            return;
        }
    }
    public override VRData GetData()
    {
        return null;
    }
}

public class PropScale : VRProperty
{
    VRVariable scaleVariable;
    public override string Name()
    {
        return "Scale";
    }
    public override bool IsType(VRObject vrObject)
    {
        // Gameobject always has a transform

        RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
        if (rect)
            return false;


        return true;
    }

    public override bool CanBeUsed()
    {
        return false;
    }
    public override void SetupVariables()
    {
        base.SetupVariables();

        DatFloat datFloat = new DatFloat(vrObject.gameObject.transform.localScale.x);
        datFloat.max = 3;

        scaleVariable = new VRVariable();

        scaleVariable.Setup(datFloat);
        scaleVariable.name = "Scale";
        scaleVariable.allowDatName = true;
        scaleVariable.OnSetData += SetData;
        scaleVariable.OnGetData += GetData;

        vrVariables.Add(scaleVariable);
    }

    public override VRData GetData()
    {
        return new DatFloat(vrObject.gameObject.transform.localScale.x);
    }

    public void SetData(VRData vrData)
    {
        DatFloat datFloat = (DatFloat)vrData;
        vrObject.gameObject.transform.localScale = Vector3.one * datFloat.Value;
    }

    public override void Trigger()
    {
        DatFloat vrFloat = (DatFloat)scaleVariable.vrData;
        vrObject.gameObject.transform.localScale = Vector3.one * vrFloat.Value;
    }
}


public class PropTransform : VRProperty
{
    // Teleport
    VRTab tabTeleport;
    VRTab tabMove;
    VRTab tabRecording;
    // Variables
    VRVariable varTransform;
    VRVariable varDuration;
    VRVariable varRecording;
    VRVariable varLoop;

    bool playing = false;
    public override string Name()
    {
        return "Transform";
    }
    public override bool IsType(VRObject vrObject)
    {
        // Gameobject always has a transform

        RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
        if (rect)
            return false;


        return true;
    }

    public override void SetupTabs()
    {
        base.SetupTabs();

        // Variables
        varTransform = new VRVariable(new DatTransform(new DatObj(vrObject)), "Transform");
        varDuration = new VRVariable(new DatFloat(1), "Duration");
        varDuration.allowDatName = true;
        varRecording = new VRVariable(new DatRecording(new DatTransform(new DatObj(vrObject))), "Recording");
        varLoop = new VRVariable(new DatBool(false), "Loop");


        // Tabs
        tabTeleport = new VRTab("Teleport");
        tabTeleport.vrVariables.Add(varTransform);
        vrTabs.Add(tabTeleport);

        tabMove = new VRTab("Move");
        tabMove.vrVariables.Add(varTransform);
        tabMove.vrVariables.Add(varDuration);
        vrTabs.Add(tabMove);


        tabRecording = new VRTab("Record");
        tabRecording.vrVariables.Add(varRecording);
        tabRecording.vrVariables.Add(varDuration);
        tabRecording.vrVariables.Add(varLoop);
        vrTabs.Add(tabRecording);
    }

    public override void Trigger()
    {
        VRTab activeTab = GetActiveTab();
        if (activeTab == null)
            return;

        Transform transform = vrObject.gameObject.transform;
        DatTransform datTransform = (DatTransform)varTransform.vrData;
        Vector3 position = datTransform.datPosition.Value;
        Quaternion rotation = datTransform.datRotation.Value;
        Vector3 localScale = datTransform.datLocalScale.Value;

        DatFloat datDuration = (DatFloat)varDuration.vrData;

        DatRecording datRecording = (DatRecording)varRecording.vrData;
        DatBool datLoop =(DatBool) varLoop.vrData;

        if (activeTab == tabTeleport)
        {
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = localScale;

            return;
        }
        if (activeTab == tabMove)
        {

            transform.DOMove(position, datDuration.Value);
            transform.DOScale(localScale, datDuration.Value);
            transform.DORotateQuaternion(rotation, datDuration.Value);
            return;
        }
        if(activeTab == tabRecording)
        {
            if (!playing)
            {
                VRManager.instance.StartCoroutine(Play(datDuration.Value, datRecording, () =>
                {
                    if (datLoop.Value)
                        Trigger();
                }));
            }
        }
    }
    private IEnumerator Play(float duration, DatRecording datRecording, Action OnComplete)
    {
        playing = true;

        List<DatTransform> recording = datRecording.Value;
        float fps = (float)recording.Count / duration;

        Transform transform = datRecording.datTransform.datObj.Value.gameObject.transform;
        Rigidbody rigid = datRecording.datTransform.datObj.Value.rigid;
        bool wasKinematic = false;
        if (rigid)
        {
            wasKinematic = rigid.isKinematic;
            rigid.isKinematic = false;
        }
        for (int i = 0; i < recording.Count; i++)
        {
            DatVector3 datPosition = recording[i].datPosition;
            DatQuaternion datRotation = recording[i].datRotation;
            DatVector3 datLocalScale = recording[i].datLocalScale;

            float stepTime = 1f / fps;
            transform.DOMove(datPosition.Value, stepTime);
            transform.DORotateQuaternion(datRotation.Value, stepTime);
            transform.DOScale(datLocalScale.Value, stepTime);
            if (rigid)
                rigid.velocity = Vector3.zero;

            yield return new WaitForSeconds(stepTime);
        }
        playing = false;
        if (rigid)
        {
            rigid.isKinematic = wasKinematic;
        }

        OnComplete?.Invoke();
    }
    public override VRData GetData()
    {
        return null;
    }
}


public class PropButton : VRProperty
{
    VRVariable varTrigger;

    Button button;
    public override string Name()
    {
        return "Button Pressed";
    }
    public override bool IsType(VRObject vrObject)
    {
        // Gameobject always has a transform
        Button button = vrObject.gameObject.GetComponentInParent<Button>();

        if (button)
            return true;
        else
            return false;
    }

    public override bool CanBeUsed()
    {
        return true;
    }
    public override void SetupVariables()
    {
        base.SetupVariables();

        button = vrObject.gameObject.GetComponentInParent<Button>();
        varTrigger = new VRVariable(new DatEvent(-1), "OnPressed");

        button.onClick.AddListener(() => 
        {
            varTrigger.SetData(new DatEvent(VRManager.tickIndex++));
        });

        vrVariables.Add(varTrigger);

    }

    public override VRData GetData()
    {
        return null;
    }
}
public class PropColor : VRProperty
{
    VRVariable varColor;


    Renderer renderer;
    Image image;
    RawImage rawImage;
    Text text;

    public override string Name()
    {
        return "";
    }
    public override bool IsType(VRObject vrObject)
    {
        return SetupColorComponent(vrObject);
    }

    public override bool CanBeUsed()
    {
        return true;
    }
    public override void SetupVariables()
    {
        base.SetupVariables();

        SetupColorComponent(vrObject);
        Color color = GetColor();

        varColor = new VRVariable(new DatColor(color), "");
        varColor.OnSetData += SetData;

        vrVariables.Add(varColor);

    }

    bool SetupColorComponent(VRObject vrObject)
    {
        GameObject gameObject = vrObject.gameObject;
        if (gameObject.TryGetComponent(out renderer))
            return true;
        if (gameObject.TryGetComponent(out image))
            return true;
        if (gameObject.TryGetComponent(out rawImage))
            return true;
        if (gameObject.TryGetComponent(out text))
            return true;

        //Couldn't find component with color
        return false;
    }

    Color GetColor()
    {
        Color color = Color.cyan;
        if (renderer)
            color = renderer.material.color;
        if (image)
            color = image.color;
        if (rawImage)
            color = rawImage.color;
        if (text)
            color = text.color;
        return color;
    }

    void SetColor(Color color)
    {
        if (renderer)
            renderer.material.color = color;
        if (image)
            image.color = color;
        if (rawImage)
            rawImage.color = color;
        if (text)
            text.color = color;
    }

    public override void Trigger()
    {
        base.Trigger();
        SetData(varColor.GetData());
    }
    public void SetData(VRData vrData)
    {
        DatColor datColor = (DatColor)vrData;
        SetColor(datColor.Value);
    }
    public override VRData GetData()
    {
        return null;
    }
}