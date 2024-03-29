using System;
using System.Collections;
using System.Collections.Generic;
using LayerSave;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace LayerBottom
{
    [System.Serializable]
    public abstract class VREvent : VRLogicElement
    {
        static List<VREvent> allEvents;
        public VREvent()
        {
            isRoot = true;
        }
        public static List<VREvent> GetAllEvents()
        {
            if (allEvents != null)
                return allEvents;


            allEvents = new List<VREvent>();
            IEnumerable<Type> subClasses = VRManager.GetAllSubclassOf(typeof(VREvent));
            foreach (Type type in subClasses)
            {
                VREvent obj = (VREvent)Activator.CreateInstance(type);
                SaveManager.RemoveSaveElement(obj);
                allEvents.Add(obj);
            }
            return allEvents;
        }

        public static VREvent GetEvent(string name)
        {
            List<VREvent> allEvents = GetAllEvents();
            foreach(VREvent vrEvent in allEvents)
            {
                if (vrEvent.Name() == name)
                    return vrEvent;
            }

            return null;
        }
    }

    [System.Serializable]
    public class WaitEvent : VREvent
    {
        [SerializeReference] VRPort outEvent;
        [SerializeReference] VRPort inEvent;

        [SerializeReference] VRVariable varDuration;

        Coroutine myWaitCoroutine;
        [SerializeField] bool isWaiting = false;

        public override string Name()
        {
            return "Wait";
        }
        public override void Setup()
        {
            base.Setup();
        }
        public override void SetupInputs()
        {
            base.SetupInputs();
            inEvent = new VRPort(this, new DatEvent(0), PortType.INPUT);
            vrInputs.Add(inEvent);
        }
        public override void SetupOutputs()
        {
            base.SetupOutputs();
            outEvent = new VRPort(this, new DatEvent(0), PortType.OUTPUT);
            vrOutputs.Add(outEvent);
        }
        public override void SetupVariables()
        {
            base.SetupVariables();
            varDuration = new VRVariable(new DatFloat(1),this, "Duration",true);
            vrVariables.Add(varDuration);
        }
        public override void SetData(VRData vrData)
        {
            if (isWaiting)
                return;

            DatFloat datDuration = (DatFloat)varDuration.vrData;
            VRManager.instance.StartCoroutine(Wait(datDuration.Value, vrData));
        }

        IEnumerator Wait(float duration, VRData vrData)
        {
            isWaiting = true;
            yield return new WaitForSeconds(duration);
            outEvent.SetData(vrData);
            isWaiting = false;
        }
    }

    [System.Serializable]
    public class EventProximity : VREvent
    {
        [SerializeReference] VRPort outEvent;
        [SerializeReference] VRPort inEvent;


        [SerializeReference] VRVariable varDistance;
        [SerializeReference] VRVariable varObjA;
        [SerializeReference] VRVariable varObjB;

        public override string Name()
        {
            return "ProximityAlert";
        }
        public override void Setup()
        {
            base.Setup();
        }
        public override void SetupInputs()
        {
            base.SetupInputs();
            inEvent = new VRPort(this, new DatEvent(0), PortType.INPUT);
            inEvent.toolTip = "Require Event";
            vrInputs.Add(inEvent);
        }
        public override void SetupOutputs()
        {
            base.SetupOutputs();
            outEvent = new VRPort(this, new DatEvent(0), PortType.OUTPUT);
            vrOutputs.Add(outEvent);
        }
        public override void SetupVariables()
        {
            base.SetupVariables();

            varDistance = new VRVariable(new DatFloat(0.1f),this,"Distance",true);
            vrVariables.Add(varDistance);

            varObjA = new VRVariable(new DatObj(null), this,"Object: ",true);
            varObjA.allowDatName = true;
            vrVariables.Add(varObjA);

            varObjB = new VRVariable(new DatObj(null), this,"Object: ", true);
            varObjB.allowDatName = true;
            vrVariables.Add(varObjB);
        }

        public override void Update(DatEvent datEvent)
        {
            if (inEvent.IsConnected())
                return;

            SetData(datEvent);
        }

        public override void SetData(VRData vrData)
        {
            DatEvent datEvent = (DatEvent)vrData;

            VRDebug.SetLog($"{Name()}: UPDATE {datEvent.Value.ToString("0.0")}");

            DatObj datObjA = (DatObj)varObjA.GetData();
            DatObj datObjB = (DatObj)varObjB.GetData();
            DatFloat datDistance = (DatFloat)varDistance.GetData();


            VRDebug.SetLog($"{Name()}: {datObjA.Value != null} {datObjB.Value != null} {datDistance.Value}");
            if (datObjA.Value == null || datObjB.Value == null)
                return;

            // Use Pivot of object
            Vector3 posA = datObjA.Value.gameObject.transform.position;
            Vector3 posB = datObjB.Value.gameObject.transform.position;

            // Use center of Renderer
            if (datObjA.Value.renderer)
                posA = datObjA.Value.renderer.bounds.center;
            if (datObjB.Value.renderer)
                posB = datObjB.Value.renderer.bounds.center;
            
            
            float distance = datDistance.Value;

            if (Vector3.Distance(posA, posB) < distance)
            {
                outEvent.SetData(datEvent);
                VRDebug.SetLog($"{Name()}: TRIGGERED");
            }
            else
            {
                VRDebug.SetLog($"{Name()}: NOT-TRIGGERED");
            }
        }
    }






    [System.Serializable]
    public class EverySecond : VREvent
    {
        [SerializeReference] VRPort outEvent;

        [SerializeReference] VRVariable varDuration;

        private float lastTime = 0;

        public override string Name()
        {
            return "Every Second";
        }
        public override void SetupOutputs()
        {
            base.SetupOutputs();
            outEvent = new VRPort(this, new DatEvent(0), PortType.OUTPUT);
            vrOutputs.Add(outEvent);
        }
        public override void SetupVariables()
        {
            base.SetupVariables();
            varDuration = new VRVariable(new DatFloat(1),this, "Seconds", true);
            vrVariables.Add(varDuration);

        }

        public override void FixedUpdate(DatEvent datEvent)
        {
            DatFloat datDuration = (DatFloat)varDuration.vrData;

            if (Time.time - lastTime > datDuration.Value)
            {
                outEvent.SetData(datEvent);
                lastTime = Time.time;
            }
        
        }
    }




    [System.Serializable]
    public class EventStart : VREvent
    {
        [SerializeReference] VRPort outEvent;

        public override string Name()
        {
            return "Start";
        }
        public override void SetupOutputs()
        {
            base.SetupOutputs();
            outEvent = new VRPort(this, new DatEvent(0), PortType.OUTPUT);
            vrOutputs.Add(outEvent);
        }

        public override void Start(DatEvent datEvent)
        {
            base.Start(datEvent);
            outEvent.SetData(datEvent);
        }
    }
    
}