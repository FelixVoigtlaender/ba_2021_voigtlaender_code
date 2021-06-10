using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class VRManager : MonoBehaviour
{
    public static VRManager instance;

    public event Action<VRObject> OnInitVRObject;
    public event Action<VREvent> OnInitVREvent;
    public event Action<VRVariable> OnInitVRVariable;
    public event Action<VRConnection> OnInitVRConnection;


    public List<VRObject> vrObjects = new List<VRObject>();
    public List<VRAction> vrActions = new List<VRAction>();
    public List<VREvent> vrEvents = new List<VREvent>();


    private void Awake()
    {
        instance = this;
    }

    public VREvent InitVREvent(string name, bool notify = true)
    {
        VREvent vrEvent = VREvent.GetEvent(name);
        if (vrEvent == null)
            return null;

        vrEvent.SetupPorts();
        vrEvents.Add(vrEvent);

        if (notify)
            OnInitVREvent?.Invoke(vrEvent);

        return vrEvent;
    }

    public VRConnection InitVRConnection(VRPort start, VRPort end, bool notify = true)
    {
        VRConnection vrConnection = new VRConnection();
        vrConnection.ConnectStart(start);
        vrConnection.ConnectEnd(end);
        //TODO Add to list

        if (notify)
            OnInitVRConnection?.Invoke(vrConnection);

        return vrConnection;
    }

    public VRObject InitVRObject(GameObject gameObject, bool notify = true)
    {
        if (FindVRObject(gameObject) != null)
            return null;

        VRObject vrObject = new VRObject();
        vrObject.Setup(gameObject);
        vrObjects.Add(vrObject);

        if (notify)
            OnInitVRObject?.Invoke(vrObject);

        return vrObject;
    }

    public VRVariable InitVRVariable(VRData vrData, bool notify = true)
    {
        VRVariable vrVariable = new VRVariable();
        vrVariable.Setup(vrData);

        if (notify)
            OnInitVRVariable?.Invoke(vrVariable);

        return vrVariable;
    }

    public VRObject FindVRObject(GameObject go)
    {
        foreach(VRObject vrObject in vrObjects)
        {
            if (vrObject.gameObject == go)
                return vrObject;
        }
        return null;
    }
}
