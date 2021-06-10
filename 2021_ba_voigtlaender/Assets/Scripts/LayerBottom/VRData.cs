using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VRData 
{
    public abstract bool IsType(VRData data);
    public abstract Color GetColor();
    public abstract string GetName();

    public Color DecimalToColor(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}

public class DatString : VRData
{
    public string value;

    public DatString(string value)
    {
        this.value = value;
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
        return value.ToString();
    }
}

public class DatFloat : VRData
{
    public float value;


    public DatFloat(float value)
    {
        this.value = value;
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
        return value.ToString();
    }
}

public class DatInt : VRData
{
    public int value;
    public DatInt(int value)
    {
        this.value = value;
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
        return value.ToString();
    }
}


public class DatVector3 : VRData
{
    public Vector3 value;
    public DatVector3(Vector3 value)
    {
        this.value = value;
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
        return value.ToString();
    }
}

public class DatEvent : VRData
{
    public float value;
    public DatEvent(float value)
    {
        this.value = value;
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
        return value.ToString();
    }
}

public class DatBool : VRData
{
    public bool value;
    public DatBool(bool value)
    {
        this.value = value;
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
        return value.ToString();
    }
}

public class DatObj : VRData
{
    public VRObject value;
    public DatObj(VRObject value)
    {
        this.value = value;
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
        return value.ToString();
    }
}

