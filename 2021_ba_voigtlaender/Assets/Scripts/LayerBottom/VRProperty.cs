using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LayerSave;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace LayerBottom
{
    [System.Serializable]
    public abstract class VRProperty : VRLogicElement
    {
        [SerializeReference] protected VRObject vrObject;
        [SerializeReference] protected VRPort output;
        [SerializeReference] protected VRPort input;

        static List<VRProperty> allProperties;

    
        public abstract bool IsType(VRObject vrObject);
        public virtual void Setup(VRObject vrObject) 
        {
            this.vrObject = vrObject;
            isActive = false;
            base.Setup();
        }


        public virtual bool CanBeUsed()
        {
            return true;
        }
        public static List<VRProperty> GetAllPorperties()
        {
            if (allProperties != null)
                return allProperties;
        


            allProperties = new List<VRProperty>();
            IEnumerable<Type> subClasses = VRManager.GetAllSubclassOf(typeof(VRProperty));
            foreach(Type type in subClasses)
            {
                VRProperty vrProperty = (VRProperty) Activator.CreateInstance(type);
                SaveManager.RemoveSaveElement(vrProperty);
                if (vrProperty.CanBeUsed())
                    allProperties.Add(vrProperty);
            }
            return allProperties;
        }

    }

    [System.Serializable]
    public class PropTrigger : VRProperty
    {
        [SerializeReference] VRVariable eventVariable;
        public override string Name()
        {
            return "Apply Changes";
        }
        public override bool IsType(VRObject vrObject)
        {
            RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
            if (rect)
                return false;
            // Gameobject always can be triggered
            return true;
        }

        public override void Setup(VRObject vrObject)
        {
            base.Setup(vrObject);
            isActive = true;
        }

        public override void SetupVariables()
        {
            base.SetupVariables();
            eventVariable = new VRVariable(new DatEvent(-1), this,"Apply Changes",false,false,true);
            vrVariables.Add(eventVariable);
        }


        public override void SetupOutputs()
        {
            base.SetupOutputs();
            output = new VRPort(this, new DatEvent(-1), PortType.OUTPUT);
            //vrOutputs.Add(output);
        }
        public override void SetupInputs()
        {
            base.SetupInputs();
            input = new VRPort(this, new DatEvent(-1), PortType.INPUT);
            //vrInputs.Add(input);
        }

        public override VRData GetData()
        {
            return new DatEvent(-1);
        }
        public override void SetData(VRData vrData)
        {
            VRDebug.Log("Applying to: " + vrObject.gameObject.name);
            vrObject.Trigger();
        }

        public override void Trigger()
        {
        }

    }

    [System.Serializable]
    public class PropEnabled : VRProperty
    {
        [SerializeReference] private VRTab activeTab;
        [SerializeReference] private VRTab inactiveTab;
    
    
        public override string Name()
        {
            return "Change Visibility";
        }
        public override bool IsType(VRObject vrObject)
        {
            RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
            if (rect)
                return false;

            // Gameobject always can be enabled
            return true;
        }


        public override void SetupTabs()
        {
            base.SetupTabs();

            activeTab = new VRTab("Visible");
            vrTabs.Add(activeTab);
        
            inactiveTab = new VRTab("Invisible");
            inactiveTab.isActive = false;
            vrTabs.Add(inactiveTab);

        }

        public override void SetupVariables()
        {
            base.SetupVariables();
        }

        public override void SetData(VRData vrData)
        {
            Trigger();
        }

        public override void Trigger()
        {
            vrObject.gameObject.SetActive(activeTab.isActive);
        }

        public override VRData GetData()
        {
            return new DatBool(activeTab.isActive);
        }
    }



    [System.Serializable]
    public class PropPhysics : VRProperty
    {

    
        private Rigidbody _rigid;

        private Rigidbody rigid
        {
            get
            {
                if (_rigid)
                    return _rigid;

                _rigid = vrObject.gameObject.GetComponent<Rigidbody>();
                if (_rigid)
                    return _rigid;

                _rigid = vrObject.gameObject.AddComponent<Rigidbody>();

                return _rigid;
            }
        }
    
        public override string Name()
        {
            return "Physics";
        }
        public override bool IsType(VRObject vrObject)
        {
            RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
            if (rect)
                return false;

            // Gameobject always can be enabled
            return true;
        }
    
        public override void SetData(VRData vrData)
        {
            Trigger();
        }

        public override void Trigger()
        {
            rigid.isKinematic = !isActive;
        }
    }



    [System.Serializable]
    public class PropWalkable : VRProperty
    {
        private TeleportationArea _tele;

        private TeleportationArea tele
        {
            get
            {
                if (_tele)
                    return _tele;

                _tele = vrObject.gameObject.GetComponent<TeleportationArea>();
                if (_tele)
                    return _tele;

                _tele = vrObject.gameObject.AddComponent<TeleportationArea>();

                return _tele;
            }
        }
    
        public override string Name()
        {
            return "Walkable";
        }
        public override bool IsType(VRObject vrObject)
        {
            RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
            if (rect)
                return false;

            // Gameobject always can be enabled
            return true;
        }
    
        public override void SetData(VRData vrData)
        {
            Trigger();
        }

        public override void Trigger()
        {
            tele.enabled = isActive;
        }
    }


    [System.Serializable]
    public class PropGrabbable : VRProperty
    {
        private XRGrabInteractable _grab;

        private XRGrabInteractable grab
        {
            get
            {
                if (_grab)
                    return _grab;

                _grab = vrObject.gameObject.GetComponent<XRGrabInteractable>();
                if (_grab)
                    return _grab;

                _grab = vrObject.gameObject.AddComponent<XRGrabInteractable>();

                return _grab;
            }
        }
    
        public override string Name()
        {
            return "Grabbable";
        }
        public override bool IsType(VRObject vrObject)
        {
            RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
            if (rect)
                return false;

            // Gameobject always can be enabled
            return true;
        }
    
        public override void SetData(VRData vrData)
        {
            Trigger();
        }

        public override void Trigger()
        {
            grab.enabled = isActive;
            if (isActive)
            {
                foreach (var property in vrObject.properties)
                {
                    if (property is PropPhysics)
                    {
                        property.isActive = true;
                        property.Trigger();
                    }
                }
            }
        
        }
    }



    [System.Serializable]
    public class PropLight : VRProperty
    {
        [SerializeReference] private VRTab settingsTab;
        [SerializeReference] private VRVariable varColor;
        [SerializeReference] private VRVariable varRange;

    
        private Light _light;

        private Light light
        {
            get
            {
                if (_light)
                    return _light;

                _light = vrObject.gameObject.GetComponent<Light>();
                if (_light)
                    return _light;

                _light = vrObject.gameObject.AddComponent<Light>();

                return _light;
            }
        }
    
        public override string Name()
        {
            return "Light";
        }
        public override bool IsType(VRObject vrObject)
        {
            RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
            if (rect)
                return false;

            // Gameobject always can be enabled
            return true;
        }

        public override void SetupTabs()
        {
            base.SetupTabs();

            settingsTab = new VRTab("Light");

            varColor = new VRVariable(new DatColor(Color.white), this,"Color", true);
            settingsTab.vrVariables.Add(varColor);

            varRange = new VRVariable(new DatFloat(10), this,"Range", true);
            settingsTab.vrVariables.Add(varRange);
        
            vrTabs.Add(settingsTab);

        }

        public override void Trigger()
        {
            light.enabled = isActive;

            DatColor datColor = (DatColor) varColor.vrData;
            DatFloat datFloat = (DatFloat) varRange.vrData;

            light.color = datColor.Value;
            light.range = datFloat.Value;
        }
    }


    [System.Serializable]
    public class PropObj : VRProperty
    {
        public override bool CanBeUsed()
        {
            return false;
        }
        public override string Name()
        {
            return "Object";
        }
        public override bool IsType(VRObject vrObject)
        {
            // Gameobject always has a transform
            return true;
        }

        public override void SetupOutputs()
        {
            base.SetupOutputs();
            output = new VRPort(this, new DatObj(new VRObject()), PortType.OUTPUT);
            vrOutputs.Add(output);
        }
        public override void SetupInputs()
        {
            base.SetupInputs();
            input = new VRPort(this, new DatObj(new VRObject()), PortType.INPUT);
            //vrInputs.Add(input);
        }

        public override VRData GetData()
        {
            VRDebug.print("GettingObject: " + vrObject.gameObject);
            return new DatObj(vrObject);
        }

        public override void Trigger()
        {
        }

    }

    [System.Serializable]
    public class PropPosition : VRProperty
    {
        [SerializeReference] VRVariable positionVariable;

        public override bool CanBeUsed()
        {
            return false;
        }

        public override string Name()
        {
            return "Position";
        }
        public override bool IsType(VRObject vrObject)
        {
            // Only world objects should be moved
            RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
            RectTransform canvas = vrObject.gameObject.GetComponent<RectTransform>();

            if (canvas)
                return true;
            if (rect)
                return false;


            return true;
        }


        public override void SetupVariables()
        {
            base.SetupVariables();
            positionVariable = new VRVariable();
            positionVariable.Setup(new DatVector3(vrObject.gameObject.transform.position),this);
            positionVariable.name = "Position";

            vrVariables.Add(positionVariable);
        }

        public override VRData GetData()
        {
            return new DatVector3(vrObject.gameObject.transform.position);
        }

        public override void SetData(VRData vrData)
        {
            DatVector3 datVector = (DatVector3)vrData;
            vrObject.gameObject.transform.position = datVector.Value;
        }

        public override void Trigger()
        {
            DatVector3 vrVector3 = (DatVector3)positionVariable.vrData;
            vrObject.gameObject.transform.position = vrVector3.Value;
        }

    }

    [System.Serializable]
    public class PropMovement : VRProperty
    {
        // Teleport
        [SerializeReference] VRTab tabTeleport;
        [SerializeReference] VRTab tabMove;

        // Variables
        [SerializeReference] VRVariable varPosition;
        [SerializeReference] VRVariable varDuration;
        public override string Name()
        {
            return "Animate";
        }
        public override bool IsType(VRObject vrObject)
        {
            RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
            RectTransform canvas = vrObject.gameObject.GetComponent<RectTransform>();

            if (canvas)
                return true;
            if (rect)
                return false;


            return true;
        }

        public override bool CanBeUsed()
        {
            return false;
        }
        public override void SetupTabs()
        {
            base.SetupTabs();

            // Variables
            varPosition = new VRVariable(new DatVector3(vrObject.gameObject.transform.position), this,"Position");
            varDuration = new VRVariable(new DatFloat(1), this,"Duration");

            // Tabs
            tabTeleport = new VRTab("Teleport");
            tabTeleport.vrVariables.Add(varPosition);
            vrTabs.Add(tabTeleport);

            tabMove = new VRTab("Move");
            tabMove.vrVariables.Add(varPosition);
            tabMove.vrVariables.Add(varDuration);
            vrTabs.Add(tabMove);
        }

        public override void Trigger()
        {
            VRTab activeTab = GetActiveTab();
            if (activeTab == null)
                return;

            if(activeTab == tabTeleport)
            {
                DatVector3 vrVector3 = (DatVector3)varPosition.vrData;
                vrObject.gameObject.transform.position = vrVector3.Value;
                return;
            }
            if (activeTab == tabMove)
            {
                DatVector3 datPosition = (DatVector3)varPosition.vrData;
                DatFloat datDuration = (DatFloat)varDuration.vrData;
                vrObject.gameObject.transform.DOMove(datPosition.Value, datDuration.Value);
                return;
            }
        }
        public override VRData GetData()
        {
            return null;
        }
    }
    [System.Serializable]
    public class PropScale : VRProperty
    {
        [SerializeReference] VRVariable scaleVariable;
        public override string Name()
        {
            return "Do Scale";
        }
        public override bool IsType(VRObject vrObject)
        {
            // Gameobject always has a transform

            RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
            if (rect)
                return false;


            return true;
        }

        public override bool CanBeUsed()
        {
            return false;
        }
        public override void SetupVariables()
        {
            base.SetupVariables();

            DatFloat datFloat = new DatFloat(vrObject.gameObject.transform.localScale.x);
            datFloat.max = 3;

            scaleVariable = new VRVariable(datFloat,this,"End Scale",true);

            vrVariables.Add(scaleVariable);
        }

        public override VRData GetData()
        {
            return new DatFloat(vrObject.gameObject.transform.localScale.x);
        }

        public override void SetData(VRData vrData)
        {
            if (!isActive)
                return;
            DatFloat datFloat = (DatFloat)vrData;
            vrObject.gameObject.transform.localScale = Vector3.one * datFloat.Value;
        }

        public override void Trigger()
        {

            if (!isActive)
                return;
            DatFloat vrFloat = (DatFloat)scaleVariable.vrData;
            vrObject.gameObject.transform.localScale = Vector3.one * vrFloat.Value;
        }
    }

    [System.Serializable]
    public class PropTransform : VRProperty
    {
        // Teleport
        [SerializeReference] VRTab tabTransform;
        [SerializeReference] VRTab tabRecording;
        // Variables
        [SerializeReference] VRVariable varTransform;
        [SerializeReference] VRVariable varDuration;
        [SerializeReference] VRVariable varRecording;
        [SerializeReference] VRVariable varLoop;

        [SerializeReference] VRVariable varLockPosition;
        [SerializeReference] VRVariable varLockRotation;
        [SerializeReference] VRVariable varLockScale;


        bool playing = false;
        public override string Name()
        {
            return "Animate";
        }
        public override bool IsType(VRObject vrObject)
        {
            // Gameobject always has a transform

            RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
            if (rect)
                return false;


            return true;
        }
        public override void SetupOutputs()
        {
            base.SetupOutputs();
            VRPort outTransform = new VRPort(this,new DatTransform(new DatObj(vrObject)), PortType.OUTPUT);
            outTransform.toolTip = "Get current Transform";

            //vrOutputs.Add(outTransform);
        }

        public override void SetupTabs()
        {
            base.SetupTabs();

            // Variables
            varTransform = new VRVariable(new DatTransform(new DatObj(vrObject)),this, "Preview",true);
            varDuration = new VRVariable(new DatFloat(1), this,"Seconds", true);
            varRecording = new VRVariable(new DatRecording(new DatTransform(new DatObj(vrObject))), this,"Recording", true);
            varLoop = new VRVariable(new DatBool(false), this,"Loop", true);

            varLockPosition = new VRVariable(new DatBool(false), this,"Lock Position", true);
            varLockRotation = new VRVariable(new DatBool(false), this,"Lock Rotation", true);
            varLockScale = new VRVariable(new DatBool(false), this,"Lock Scale", true);

            // Tabs

            tabTransform = new VRTab("Slide");
            tabTransform.vrVariables.Add(varTransform);
            tabTransform.vrVariables.Add(varDuration);
            tabTransform.vrVariables.Add(varLockPosition);
            tabTransform.vrVariables.Add(varLockRotation);
            tabTransform.vrVariables.Add(varLockScale);
            vrTabs.Add(tabTransform);


            tabRecording = new VRTab("Animate");
            tabRecording.vrVariables.Add(varRecording);
            tabRecording.vrVariables.Add(varDuration);
            tabRecording.vrVariables.Add(varLoop);
            tabTransform.isActive = false;
            vrTabs.Add(tabRecording);

        }

        public override void Trigger()
        {
            if (!isActive)
                return;

            VRTab activeTab = GetActiveTab();
            if (activeTab == null)
                return;

            Transform transform = vrObject.gameObject.transform;
            DatTransform datTransform = (DatTransform)varTransform.GetData();
            Vector3 position = datTransform.datPosition.Value;
            Quaternion rotation = datTransform.datRotation.Value;
            Vector3 localScale = datTransform.datLocalScale.Value;

            DatFloat datDuration = (DatFloat)varDuration.GetData();

            DatRecording datRecording = (DatRecording)varRecording.GetData();
            DatBool datLoop =(DatBool) varLoop.GetData();
            DatBool datLockPosition =(DatBool) varLockPosition.GetData();
            DatBool datLockRotation =(DatBool) varLockRotation.GetData();
            DatBool datLockScale =(DatBool) varLockScale.GetData();



            if (activeTab == tabTransform)
            {
                if(!datLockPosition.Value)
                    transform.DOMove(position, datDuration.Value);
                if (!datLockRotation.Value)
                    transform.DORotateQuaternion(rotation, datDuration.Value);
                if (!datLockScale.Value)
                    transform.DOScale(localScale, datDuration.Value);
                return;
            }
            if(activeTab == tabRecording)
            {
                if (!playing)
                {
                    VRManager.instance.StartCoroutine(Play(datDuration.Value, datRecording, () =>
                    {
                        if (datLoop.Value && GetActiveTab() == tabRecording)
                            Trigger();
                    }));
                }
            }
        }
        private IEnumerator Play(float duration, DatRecording datRecording, Action OnComplete)
        {
            playing = true;

            List<DatTransform> recording = datRecording.Value;
            float fps = (float)recording.Count / duration;

            Transform transform = datRecording.datTransform.datObj.Value.gameObject.transform;
            Rigidbody rigid = datRecording.datTransform.datObj.Value.rigid;
            bool wasKinematic = false;
            if (rigid)
            {
                wasKinematic = rigid.isKinematic;
                rigid.isKinematic = false;
            }
            for (int i = 0; i < recording.Count; i++)
            {
                DatVector3 datPosition = recording[i].datPosition;
                DatQuaternion datRotation = recording[i].datRotation;
                DatVector3 datLocalScale = recording[i].datLocalScale;

                float stepTime = 1f / fps;
                transform.DOMove(datPosition.Value, stepTime);
                transform.DORotateQuaternion(datRotation.Value, stepTime);
                transform.DOScale(datLocalScale.Value, stepTime);
                if (rigid)
                    rigid.velocity = Vector3.zero;

                yield return new WaitForSeconds(stepTime);
            }
            playing = false;
            if (rigid)
            {
                rigid.isKinematic = wasKinematic;
            }

            OnComplete?.Invoke();
        }
        public override VRData GetData()
        {
            return new DatTransform(new DatObj(vrObject));
        }
    }

    [System.Serializable]
    public class PropButton : VRProperty
    {
        [SerializeReference] VRVariable varTrigger;

        [SerializeReference] Button button;
        public override string Name()
        {
            return "Button Pressed";
        }
        public override bool IsType(VRObject vrObject)
        {
            // Gameobject always has a transform
            Button button = vrObject.gameObject.GetComponentInParent<Button>();

            if (button)
                return true;
            else
                return false;
        }

        public override bool CanBeUsed()
        {
            return true;
        }
        public override void SetupVariables()
        {
            base.SetupVariables();

            button = vrObject.gameObject.GetComponentInParent<Button>();
            varTrigger = new VRVariable(new DatEvent(-1), this,"OnPressed",false,true);

            button.onClick.AddListener(() => 
            {
                if (!isActive)
                    return;
                varTrigger.SetData(new DatEvent(VRManager.tickIndex++));
            });

            vrVariables.Add(varTrigger);

        }

        public override VRData GetData()
        {
            return null;
        }
    }
    [System.Serializable]
    public class PropColor : VRProperty
    {
        [SerializeReference] VRVariable varColor;

        [SerializeReference] VRTab tabColor;

        Renderer renderer;
        Image image;
        RawImage rawImage;
        Text text;

        public override string Name()
        {
            return "Change Color";
        }
        public override bool IsType(VRObject vrObject)
        {

            RectTransform rect = vrObject.gameObject.GetComponent<RectTransform>();
            if (rect)
                return false;

            return SetupColorComponent(vrObject);
        }

        public override bool CanBeUsed()
        {
            return true;
        }
        public override void SetupOutputs()
        {
            base.SetupOutputs();
            VRPort outColor = new VRPort(this, new DatColor(Color.white), PortType.OUTPUT);
            outColor.toolTip = "Get current Color";
            //vrOutputs.Add(outColor);
        }
        public override void SetupTabs()
        {
            base.SetupTabs();



            SetupColorComponent(vrObject);
            Color color = GetColor();
            varColor = new VRVariable(new DatColor(color),this, "",true);

            tabColor = new VRTab("Change Color");
            tabColor.vrVariables.Add(varColor);
            vrTabs.Add(tabColor);


        }

        bool SetupColorComponent(VRObject vrObject)
        {
            GameObject gameObject = vrObject.gameObject;
            if (gameObject.TryGetComponent(out renderer))
                return true;
            if (gameObject.TryGetComponent(out image))
                return true;
            if (gameObject.TryGetComponent(out rawImage))
                return true;
            if (gameObject.TryGetComponent(out text))
                return true;

            //Couldn't find component with color
            return false;
        }

        Color GetColor()
        {
            Color color = Color.cyan;
            if (renderer)
                color = renderer.material.color;
            if (image)
                color = image.color;
            if (rawImage)
                color = rawImage.color;
            if (text)
                color = text.color;
            return color;
        }

        void SetColor(Color color)
        {
            if (renderer)
                renderer.material.color = color;
            if (image)
                image.color = color;
            if (rawImage)
                rawImage.color = color;
            if (text)
                text.color = color;
        }

        public override void Trigger()
        {
            base.Trigger();
            SetData(varColor.GetData());
        }
        public override void SetData(VRData vrData)
        {
            if (!isActive)
                return;

            VRTab activeTab = GetActiveTab();
            if(activeTab == tabColor)
            {
                DatColor datColor = (DatColor)vrData;
                SetColor(datColor.Value);
            }

        }
        public override VRData GetData()
        {
            return new DatColor(GetColor());
        }
    }
}