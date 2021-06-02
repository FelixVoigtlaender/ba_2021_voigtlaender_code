using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VRConnection : MonoBehaviour
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
        this.start = port;

        OnPortChanged?.Invoke();
    }

    public void ConnectEnd(VRPort port)
    {
        this.end = port;

        OnPortChanged?.Invoke();
    }

    public bool CanConnect(VRPort port)
    {
        if (start != null && end != null)
            return false;
        if (start == null && start == null)
            return true;

        VRPort activePort = GetActivePort();

        return port.CanConnect(activePort.dataType);
        
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
