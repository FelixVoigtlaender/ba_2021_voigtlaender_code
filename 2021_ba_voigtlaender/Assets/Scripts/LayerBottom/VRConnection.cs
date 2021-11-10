using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class VRConnection : SaveElement
{

    [SerializeReference] public VRPort start;
    [SerializeReference] public VRPort end;
    
    
    public event Action OnPortChanged;
    public event Action<VRData> OnActive;

    int lastTick = 0;

    public VRConnection()
    {
        isRoot = true;
    }
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

        if (start == null || !CheckTick())
            return null;

        VRData vrData = start.GetData();
        OnActive?.Invoke(vrData);


        return vrData;
    }
    public void SetData(VRData data)
    {
        if (end == null || !CheckTick())
            return;

        OnActive?.Invoke(data);

        end.SetData(data);
    }

    public bool CheckTick()
    {

        if (lastTick == VRManager.tickIndex)
        {
            OnActive?.Invoke(null);
            return false;
        }
        lastTick = VRManager.tickIndex;
        return true;
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

        start = portA.portType == PortType.OUTPUT ? portA : portB;
        end = portA.portType == PortType.INPUT ? portA : portB;

        start.AddConnection(this);
        end.AddConnection(this);

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

    public override void Delete()
    {
        Debug.Log("DEEEELLLEEETEEE");
        start?.RemoveConnection(this);
        end?.RemoveConnection(this);

        start = end = null;

        base.Delete();
    }
}
