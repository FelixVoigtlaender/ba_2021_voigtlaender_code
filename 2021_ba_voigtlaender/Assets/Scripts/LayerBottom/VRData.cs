using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VRData 
{
    public abstract bool IsType(VRData data);
    public abstract Color GetColor();

    public Color DecimalToColor(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}

public class VRString : VRData
{
    public string value;


    public override bool IsType(VRData data)
    {
        return data is VRString;
    }
    public override Color GetColor()
    {
        //Magenta
        return DecimalToColor(255, 0, 255);
    }
}

public class VRFloat : VRData
{
    public float value;
    public override bool IsType(VRData data)
    {
        return data is VRFloat;
    }
    public override Color GetColor()
    {
        //Yellow green
        return DecimalToColor(154, 205, 50);
    }
}

public class VRInt : VRData
{
    public int value;
    public override bool IsType(VRData data)
    {
        return data is VRInt;
    }
    public override Color GetColor()
    {
        //Sea green
        return DecimalToColor(46, 139, 87);
    }
}


public class VRVector3 : VRData
{
    public Vector3 value;
    public override bool IsType(VRData data)
    {
        return data is VRVector3;
    }
    public override Color GetColor()
    {
        //Gold
        return DecimalToColor(255, 215, 0);
    }
}

public class VREventDat : VRData
{
    public override bool IsType(VRData data)
    {
        return data is VREventDat;
    }
    public override Color GetColor()
    {
        //White
        return DecimalToColor(255, 255, 255);
    }
}

public class VRBool : VRData
{
    public bool value;
    public override bool IsType(VRData data)
    {
        return data is VRBool;
    }
    public override Color GetColor()
    {
        //Maroon
        return DecimalToColor(128, 0, 0);
    }
}

