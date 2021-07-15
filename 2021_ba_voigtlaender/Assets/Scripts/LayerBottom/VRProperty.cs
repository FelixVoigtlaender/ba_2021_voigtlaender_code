using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public static List<VRProperty> GetAllPorperties()
    {
        if (allProperties != null)
            return allProperties;


        allProperties = new List<VRProperty>();
        IEnumerable<Type> subClasses = VRManager.GetAllSubclassOf(typeof(VRProperty));
        foreach(Type type in subClasses)
        {
            VRProperty vrProperty = (VRProperty) Activator.CreateInstance(type);
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
        eventVariable.OnVariableChanged+= SetData;

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

    public override string Name()
    {
        return "Position";
    }
    public override bool IsType(VRObject vrObject)
    {
        // Gameobject always has a transform
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


public class PropTransform : VRProperty
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
        // Gameobject always has a transform
        return true;
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
        return true;
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