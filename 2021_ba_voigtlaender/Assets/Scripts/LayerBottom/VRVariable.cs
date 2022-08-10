using UnityEngine;

namespace LayerBottom
{
    [System.Serializable]
    public class VRVariable : VRLogicElement
    {
        public string name = "";
        public bool allowDatName = false;
        [SerializeReference] protected VRPort output;
        [SerializeReference] protected VRPort input;

        [SerializeReference] public VRData vrData;
        [SerializeReference] private VRLogicElement mainElement;
    
    
        public bool blockPorts = false;
        public bool blockInputs = false;
        public bool blockOutputs = false;

        //public event Action<VRData> OnVariableChanged;
        //public event Action<VRData> OnSetData;
        //public event Func<VRData> OnGetData;
        public override string Name()
        {
            if (name.Length > 0)
            {
                return allowDatName ? name + " " + vrData.GetName() : name;

            }
            return vrData.GetName();
        }
        public VRVariable() { }
        public VRVariable(VRData vrData, VRLogicElement mainElement, string name = "", bool blockPorts = false, bool blockInputs = false, bool blockOutputs = false)
        {

            this.blockPorts = blockPorts;
            this.blockInputs = blockInputs;
            this.blockOutputs = blockOutputs;
            this.name = name;

            Setup(vrData, mainElement);
        }
        public void Setup(VRData vrData, VRLogicElement mainElement)
        {
            this.vrData = vrData;
            this.mainElement = mainElement;
            base.Setup();
        
            //OnVariableChanged?.Invoke(vrData);
        }
        public override void SetupOutputs()
        {
            base.SetupOutputs();


            output = new VRPort(this, vrData,PortType.OUTPUT);
            if(name.Length != 0)
                output.toolTip = "Get " + name;
            vrOutputs.Add(output);


            if (blockPorts || blockOutputs)
                vrOutputs.Clear();
        }
        public override void SetupInputs()
        {
            base.SetupInputs();


            input = new VRPort(this, vrData, PortType.INPUT);
            input.OnConnect += () => GetData();
            if (name.Length != 0)
                input.toolTip = "Set " + name;
            vrInputs.Add(input);


            if (blockPorts || blockInputs)
                vrInputs.Clear();
        }

        public override VRData GetData()
        {
            if (input.IsConnected())
            {
                vrData.SetData(input.GetData());
                return vrData;
            }
            return vrData;
        }

        public bool IsInputConnected()
        {
            return input.IsConnected();
        }

        public bool IsOutputConnected()
        {
            return output.IsConnected();
        }
        public override void SetData(VRData vrData)
        {
            vrData.SetData(vrData);
            mainElement.SetData(vrData);
            
            if(output!= null)
                output.SetData(vrData);
        }
    }
}
