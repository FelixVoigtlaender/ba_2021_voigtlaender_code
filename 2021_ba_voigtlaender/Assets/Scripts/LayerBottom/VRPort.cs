using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VRPort : SaveElement
{
    public PortType portType = PortType.INPUT;
    [SerializeReference] public VRData dataType;
    //public VRConnection connection;
    [SerializeReference] public List<VRConnection> connections = new List<VRConnection>();
    public string toolTip = "";

    public event Func<VRData> GetElementData;
    public event Action<VRData> SetElementData;
    public event Action OnConnect;
    [SerializeReference] VRLogicElement element;
    public VRPort(Func<VRData> GetElementData, VRData dataType)
    {
        this.GetElementData = GetElementData;
        this.portType = PortType.OUTPUT;
        this.dataType = dataType;
    }
    public VRPort(VRLogicElement element, VRData dataType)
    {
        this.element = element;
        this.portType = PortType.INPUT;
        this.dataType = dataType;
    }
    public VRPort(Action<VRData> SetElementData, VRData dataType)
    {
        this.SetElementData = SetElementData;
        this.portType = PortType.INPUT;
        this.dataType = dataType;
    }

    public bool IsConnected()
    {
        return connections.Count != 0;
    }

    public VRData GetData()
    {
        if (connections.Count == 0)
            return null;

        VRData data = null;
        switch (portType)
        {
            case PortType.INPUT:
                data = connections[0].GetData();
                break;
            case PortType.OUTPUT:
                data = GetElementData();
                break;
        }

        return data;
    }
    public void SetData(VRData data)
    {
        if (connections.Count == 0)
            return;

        switch (portType)
        {
            case PortType.INPUT:
                SetElementData(data);
                break;
            case PortType.OUTPUT:
                foreach(VRConnection connection in connections)
                {

                    connection.SetData(data);
                }
                break;
        }
    }

    public void Trigger()
    {
        if (portType == PortType.INPUT && element != null)
            element.Trigger();
        if (portType == PortType.OUTPUT && connections.Count == 0)
        {
            foreach (VRConnection connection in connections)
            {
                connection.Trigger();
            }
        }
    }


    public bool CanConnect(VRData data)
    {
        return dataType.IsType(data);
    }

    public void Detach()
    {

        List<VRConnection> oldConnections = new List<VRConnection>(connections);
        connections.Clear();
        foreach (VRConnection connection in oldConnections)
        {
            connection?.Delete();
        }

    }
    public override void Delete()
    {
        Detach();
        base.Delete();
    }
    public void RemoveConnection(VRConnection connection)
    {
        connections.Remove(connection);
    }
    public void AddConnection(VRConnection connection)
    {
        connections.Add(connection);
        OnConnect?.Invoke();
    }
}

public enum PortType
{
    INPUT, OUTPUT
}