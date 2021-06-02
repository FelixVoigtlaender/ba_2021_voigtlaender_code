using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VRPort
{
    public PortType type = PortType.INPUT;
    public VRData dataType;
    public VRConnection connection;
    public event Func<VRData> GetElementData;
    public event Action<VRData> SetElementData;
    VRLogicElement element;
    public VRPort(Func<VRData> GetElementData, VRData dataType)
    {
        this.GetElementData = GetElementData;
        this.type = PortType.OUTPUT;
        this.dataType = dataType;
    }
    public VRPort(VRLogicElement element, VRData dataType)
    {
        this.element = element;
        this.type = PortType.INPUT;
        this.dataType = dataType;
    }
    public VRPort(Action<VRData> SetElementData, VRData dataType)
    {
        this.SetElementData = SetElementData;
        this.type = PortType.INPUT;
        this.dataType = dataType;
    }

    public bool IsConnected()
    {
        return connection != null;
    }

    public VRData GetData()
    {
        if (!connection)
            return null;

        VRData data = null;
        if (connection.end == this)
            data = connection.GetData();
        if (connection.start == this && GetElementData!=null)
            data = GetElementData();

        return data;
    }
    public void SetData(VRData data)
    {

        if (!connection)
            return;

        if (connection.end == this && SetElementData != null)
            SetElementData(data);
        if (connection.start == this)
            connection.SetData(data);
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