using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public abstract class VRData 
{
    public abstract bool IsType(VRData data);
    public abstract Color GetColor();
    public abstract string GetName();
    public abstract void SetData(VRData data);

    public event Action<VRData> OnDataChanged;
    protected void DataChanged()
    {
        OnDataChanged?.Invoke(this);
    }
    public Color DecimalToColor(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}

[System.Serializable]
public class DatString : VRData
{
    private string value;
    public string Value 
    {
        get { return value; }
        set { this.value = value; DataChanged(); }
    }

    public DatString(string value)
    {
        this.Value = value;
    }


    public override bool IsType(VRData data)
    {
        return data is DatString;
    }
    public override Color GetColor()
    {
        //Magenta
        return DecimalToColor(255, 0, 255);
    }

    public override string GetName()
    {
        return Value.ToString();
    }

    public override void SetData(VRData data)
    {
        Value = ((DatString)data).Value;
    }
}

[System.Serializable]
public class DatFloat : VRData
{
    private float value;
    public float Value
    {
        get { return value; }
        set { this.value = value; DataChanged(); }
    }
    public bool useMinMax = true;
    public float min = 0;
    public float max = 10;

    public DatFloat(float value)
    {
        this.Value = value;
    }
    public override bool IsType(VRData data)
    {
        return data is DatFloat;
    }
    public override Color GetColor()
    {
        //Yellow green
        return DecimalToColor(154, 205, 50);
    }
    public override string GetName()
    {
        return Value.ToString("0.00");
    }
    public override void SetData(VRData data)
    {
        Value = ((DatFloat)data).Value;
    }
}

[System.Serializable]
public class DatInt : VRData
{
    private int value;
    public int Value
    {
        get { return value; }
        set { this.value = value; DataChanged(); }
    }
    public DatInt(int value)
    {
        this.Value = value;
    }
    public override bool IsType(VRData data)
    {
        return data is DatInt;
    }
    public override Color GetColor()
    {
        //Sea green
        return DecimalToColor(46, 139, 87);
    }
    public override string GetName()
    {
        return Value.ToString();
    }
    public override void SetData(VRData data)
    {
        Value = ((DatInt)data).Value;
    }
}


[System.Serializable]
public class DatVector3 : VRData
{
    private Vector3 value;
    public Vector3 Value
    {
        get { return value; }
        set { this.value = value; DataChanged(); }
    }
    public DatVector3(Vector3 value)
    {
        this.Value = value;
    }
    public override bool IsType(VRData data)
    {
        return data is DatVector3;
    }
    public override Color GetColor()
    {
        //Gold
        return DecimalToColor(255, 215, 0);
    }
    public override string GetName()
    {
        return Value.ToString();
    }
    public override void SetData(VRData data)
    {
        Value = ((DatVector3)data).Value;
    }
}

[System.Serializable]
public class DatEvent : VRData
{
    private float value;
    public float Value
    {
        get { return value; }
        set { this.value = value; DataChanged(); }
    }
    public DatEvent(float value)
    {
        this.Value = value;
    }
    public override bool IsType(VRData data)
    {
        return data is DatEvent;
    }
    public override Color GetColor()
    {
        //White
        return DecimalToColor(255, 255, 255);
    }
    public override string GetName()
    {
        return "Event: " + Value.ToString();
    }
    public override void SetData(VRData data)
    {
        Value = ((DatEvent)data).Value;
    }
}

[System.Serializable]
public class DatBool : VRData
{
    private bool value;
    public bool Value
    {
        get { return value; }
        set { this.value = value; DataChanged(); }
    }
    public DatBool(bool value)
    {
        this.Value = value;
    }
    public override bool IsType(VRData data)
    {
        return data is DatBool;
    }
    public override Color GetColor()
    {
        //Maroon
        return DecimalToColor(128, 0, 0);
    }
    public override string GetName()
    {
        return Value.ToString();
    }
    public override void SetData(VRData data)
    {
        Value = ((DatBool)data).Value;
    }
}

[System.Serializable]
public class DatObj : VRData
{
    private VRObject value;
    public VRObject Value
    {
        get { return value; }
        set { this.value = value; DataChanged(); }
    }
    public DatObj(VRObject value)
    {
        this.Value = value;
    }
    public override bool IsType(VRData data)
    {
        return data is DatObj;
    }
    public override Color GetColor()
    {
        //Maroon
        return DecimalToColor(0, 0, 0);
    }
    public override string GetName()
    {
        if (Value == null)
            return "NULL";
        if (Value.gameObject == null)
            return "NULL-OBJ";
        return Value.gameObject.ToString();
    }
    public override void SetData(VRData data)
    {
        Value = ((DatObj)data).Value;
    }
}

