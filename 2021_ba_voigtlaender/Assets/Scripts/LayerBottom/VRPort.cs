using System;
using System.Collections;
using System.Collections.Generic;
using LayerSave;
using UnityEngine;

namespace LayerBottom
{
    [System.Serializable]
    public class VRPort : SaveElement
    {
        public PortType portType = PortType.INPUT;
        [SerializeReference] public VRData dataType;
        [SerializeReference] public List<VRConnection> connections = new List<VRConnection>();
        [SerializeReference] VRLogicElement element;
    
    
        public string toolTip = "";

        public event Action OnConnect;

        public VRPort(VRLogicElement element, VRData dataType, PortType portType)
        {
            this.element = element;
            this.dataType = dataType;
            this.portType = portType;
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
                    data = element.GetData();
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
                    element.SetData(data);
                    break;
                case PortType.OUTPUT:
                    foreach(VRConnection connection in connections)
                    {
                        connection.SetData(data);
                    }
                    break;
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
            if(connections.Contains(connection))
                return;
        
            connections.Add(connection);
            OnConnect?.Invoke();
        }
    }

    public enum PortType
    {
        INPUT, OUTPUT
    }
}