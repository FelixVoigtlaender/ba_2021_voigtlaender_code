using System.Collections.Generic;
using LayerBottom;
using UnityEngine;
using UnityEngine.UI;

namespace LayerTop
{
    public class VisObject : MonoBehaviour
    {
        VRObject vrObject;
        private List<VisProperty> visProperties = new List<VisProperty>();
        public List<VisPort> visInPorts;
        public List<VisPort> visOutPorts;

        public Transform propertyHolder;
        public RectTransform inputHolder;
        public RectTransform outputHolder;
        public RectTransform secondaryInputHolder;
        public RectTransform secondaryOutputHolder;

        public RectTransform tabHolder;

        public CanvasGroup blockProperties;

        public Text textName;
        public TooltipContent tooltipName;
        public BezierCurve lineToObject;
        public float hoverDistance = 2;
        public MiniatureMaker miniature;
        public GhostObject ghostObject;

        public Dropdown dropdown;

        public Canvas canvas;
        public RectTransform rectTransform;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            vrObject.position = canvas.transform.position;
        }

        public void Setup(VRObject vrObject)
        {
            ghostObject = VisManager.instance.DemandGhostObject(new DatTransform(new DatObj(vrObject)));
            ghostObject.gameObject.SetActive(false); 

            canvas.transform.position = vrObject.position + Vector3.up*hoverDistance;

            this.vrObject = vrObject;
            textName.text ="Object: "+ vrObject.gameObject.name;
            if (tooltipName)
                tooltipName.description = vrObject.gameObject.name;
            vrObject.OnDelete += OnDelete;

            miniature.CopyMesh(vrObject.gameObject);

            lineToObject.start.Connect(vrObject.gameObject.transform);
            lineToObject.start.useLocalSpace = false;
            lineToObject.start.dynamicNormals = false;
            lineToObject.start.normal = Vector3.up;

            lineToObject.end.Connect(canvas.transform);
            lineToObject.end.useLocalSpace = false;
            lineToObject.end.dynamicNormals = false;
            lineToObject.end.normal = Vector3.down;

            PopulateProperties();
            PopulateVisPorts(vrObject.vrInputs,vrObject.vrOutputs);

        
            dropdown.onValueChanged.AddListener(OnPropertySelected);

            OnPropertyChanged(false);
        }

        public void PopulateProperties()
        {

            RectTransform layoutRect = propertyHolder.GetComponent<RectTransform>();
            foreach(VRProperty vrProperty in vrObject.properties)
            {
                GameObject propertyPrefab = VisManager.instance.GetVisPropertyPrefab(vrProperty);
                if (!propertyPrefab)
                    continue;

                GameObject propertyObj = Instantiate(propertyPrefab, propertyHolder);
                VisProperty visProperty = propertyObj.GetComponent<VisProperty>();
                visProperty.Setup(vrProperty);
                visProperties.Add(visProperty);

                vrProperty.OnActiveChanged += OnPropertyChanged;
                vrProperty.OnActiveChanged += (value) =>
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRect);
                };
            }
        }

        public void OnPropertyChanged(bool value)
        {
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

            Dropdown.OptionData noneOption = new Dropdown.OptionData("Add Action");
            options.Add(noneOption);
        
            foreach(VRProperty vrProperty in vrObject.properties)
            {
                if (!vrProperty.isActive)
                {
                    Dropdown.OptionData propOption = new Dropdown.OptionData(vrProperty.Name());
                    options.Add(propOption);
                }
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
        
        }

        public void OnPropertySelected(int value)
        {
            Dropdown.OptionData selection = dropdown.options[value];
            foreach(VRProperty vrProperty in vrObject.properties)
            {
                if (vrProperty.Name() == selection.text)
                {
                    vrProperty.isActive = true;
                    return;
                }
            }
        }

        public void PopulateVisPorts(List<VRPort> inputs, List<VRPort> outputs)
        {
            //Inputs
            visInPorts = PopulateVisPort(inputHolder, inputs);
            visOutPorts = PopulateVisPort(outputHolder, outputs);


            visInPorts.AddRange(PopulateVisPort(secondaryInputHolder, inputs));
            visOutPorts.AddRange(PopulateVisPort(secondaryOutputHolder, outputs));
        }
        List<VisPort> PopulateVisPort(Transform holder, List<VRPort> ports)
        {
            GameObject prefabVisPort = VisManager.instance.prefabVisPort;
            List<VisPort> visPorts = new List<VisPort>();
            foreach (VRPort vrPort in ports)
            {
                GameObject objVisPort = Instantiate(prefabVisPort, holder);
                VisPort visPort = objVisPort.GetComponent<VisPort>();
                visPort.Setup(vrPort);
                visPorts.Add(visPort);
            }
            return visPorts;
        }
        public void Delete()
        {
            vrObject.Delete();
        }
        public void OnDelete()
        {
            Canvas rootCanvas = GetComponentInParent<Canvas>();
            if(rootCanvas && rootCanvas.gameObject)
            {
                Destroy(rootCanvas.gameObject);
            }

            Destroy(ghostObject.gameObject);
        }
    }
}
