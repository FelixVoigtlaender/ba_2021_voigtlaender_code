using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPort
{
    public PortType type = PortType.INPUT;
    public VRData dataType;
    public VRLogicElement element;
    public VRConnection connection;
    public VRPort(VRLogicElement element, PortType type, VRData dataType)
    {
        this.element = element;
        this.type = type;
        this.dataType = dataType;
    }


    public VRData GetData()
    {
        if (!connection)
            return null;

        VRData data = null;
        if (connection.end == this)
            data = connection.GetData();
        if (connection.start == this)
            data = element.GetData();

        return data;
    }

    public void Trigger()
    {
        if (type == PortType.INPUT && element != null)
            element.Trigger();
        if (type == PortType.OUTPUT && connection != null)
            connection.Trigger();
    }


    public bool CanConnect(VRData data)
    {
        if (this.connection != null)
            return false;

        return dataType.IsType(data);
    }
}

public enum PortType
{
    INPUT, OUTPUT
}