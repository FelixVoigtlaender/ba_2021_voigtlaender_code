using System;
using System.Collections.Generic;
using LayerSave;
using UnityEngine;

namespace LayerBottom
{
    public class VRManager : MonoBehaviour
    {
        public static VRManager instance;

        public event Action<VRObject> OnInitVRObject;
        public event Action<VREvent> OnInitVREvent;
        public event Action<VRVariable> OnInitVRVariable;
        public event Action<VRConnection> OnInitVRConnection;
        public event Action<VRAction> OnInitVRAction;
        public static int tickIndex = 0;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if(SaveManager.instance.programm==null)
                return;

            DatEvent datEvent = new DatEvent(tickIndex++);

            SaveManager.instance.programm.Update(datEvent);

        }
    
    
        private void FixedUpdate()
        {
            if(SaveManager.instance.programm==null)
                return;
        
            DatEvent datEvent = new DatEvent(tickIndex++);
        
            SaveManager.instance.programm.FixedUpdate(datEvent);

        }

        public VREvent InitVREvent(string name, bool notify = true)
        {
            VREvent vrEvent = VREvent.GetEvent(name);
            if (vrEvent == null)
                return null;


            if (notify)
                OnInitVREvent?.Invoke(vrEvent);

            return vrEvent;
        }
        public VRAction InitVRAction(string name, bool notify = true)
        {
            VRAction vrAction = VRAction.GetAction(name);
            if (vrAction == null)
                return null;

            vrAction.Setup();

            if (notify)
                OnInitVRAction?.Invoke(vrAction);

            return vrAction;
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

        public VRObject InitVRObject(GameObject gameObject, Vector3 position, bool notify = true)
        {

            VRObject vrObject = new VRObject();
            vrObject.Setup(gameObject);
            vrObject.position = position;

            if (notify)
            {
                OnInitVRObject?.Invoke(vrObject);
                vrObject.Save();
            }

            return vrObject;
        }

        public VRVariable InitVRVariable(VRData vrData, bool notify = true)
        {
            VRVariable vrVariable = new VRVariable();
            //vrVariable.Setup(vrData);

            if (notify)
                OnInitVRVariable?.Invoke(vrVariable);

            return vrVariable;
        }

        public static IEnumerable<Type> GetAllSubclassOf(Type parent)
        {
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var t in a.GetTypes())
                if (t.IsSubclassOf(parent)) yield return t;
        }
    }
}
