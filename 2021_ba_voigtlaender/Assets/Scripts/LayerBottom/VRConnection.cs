using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VRConnection 
{
    public event Action OnPortChanged;

    public VRPort start;
    public VRPort end;

    public VRPort GetOtherPort(VRPort myPort)
    {
        VRPort otherPort = null;
        if (start == myPort)
            otherPort = end;
        if (end == myPort)
            otherPort = start;
        return otherPort;
    }


    public VRData GetData()
    {
        if (start == null)
            return null;

        return start.GetData();
    }
    public void SetData(VRData data)
    {
        if (end == null)
            return;
        end.SetData(data);
    }
    
    public void Trigger()
    {
        if (end == null)
            return;
    }


    public void ConnectStart(VRPort port)
    {
        if (start == port)
            return;

        this.start = port;

        OnPortChanged?.Invoke();
    }

    public void ConnectEnd(VRPort port)
    {
        if (end == port)
            return;

        this.end = port;

        OnPortChanged?.Invoke();
    }

    public bool Connect(VRPort portA)
    {
        if (!CanConnect(portA))
            return false;
        
        //TODO

        return true;
    }
    public bool Connect(VRPort portA, VRPort portB)
    {
        if (!CanConnect(portA, portB))
            return false;
        if (!portA.CanConnect(portB.dataType))
            return false;
        if (!portA.CanConnect(portB.dataType))
            return false;

        start = portA.type == PortType.OUTPUT ? portA : portB;
        end = portA.type == PortType.INPUT ? portA : portB;

        start.connection = end.connection = this;

        return true;
    }

    public bool CanConnect(VRPort portA, VRPort portB)
    {
        if (portA == null && portB == null)
            return false;
        if (portA == null ^ portB == null)
            return true;
        if (!portA.CanConnect(portB.dataType))
            return false;
        if (!portB.CanConnect(portA.dataType))
            return false;

        return true;
    }

    public bool CanConnect(VRPort port)
    {
        if (GetActivePort() == null)
            return true;

        return CanConnect(GetActivePort(), port);   
    }

    public VRPort GetActivePort()
    {

        if (start != null)
            return start;

        if (end != null)
            return end;

        return null;
    }
}
