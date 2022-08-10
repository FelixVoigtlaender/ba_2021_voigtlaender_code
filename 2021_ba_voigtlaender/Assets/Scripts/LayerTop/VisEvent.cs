using System;
using System.Collections.Generic;
using LayerBottom;
using UnityEngine;

namespace LayerTop
{
    public class VisEvent : VisLogicElement
    {
        public VREvent vrEvent;
        public string eventName;
        event Action onFixedUpdate;

        public override bool IsType(VRLogicElement vrLogicElement)
        {
            return vrLogicElement is VREvent;
        }

        public override void Init()
        {
            if (vrEvent != null)
                return;

            vrEvent = VRManager.instance.InitVREvent(eventName,false);
            Setup(vrEvent);
        }
        public override void Setup(VRLogicElement element)
        {
            base.Setup(element);
            vrEvent = (VREvent)element;

            SetupTypes(vrEvent);
        }

        private void FixedUpdate()
        {
            onFixedUpdate?.Invoke();
        }

        public void SetupTypes(VREvent vrEvent)
        {
            switch (vrEvent)
            {
                case EventProximity eventProximity:
                    // Setup Debug
                    GameObject spherePrefab = VisManager.instance.prefabDebugSphere;
                    GameObject sphereObj = Instantiate(spherePrefab);
                    sphereObj.SetActive(false);

                    GameObject lineObj = Instantiate(VisManager.instance.prefabDebugLine);
                    LineRenderer line = lineObj.GetComponent<LineRenderer>();
                    lineObj.SetActive(false);

                    //Setup Variables
                    List<VRVariable> varObjs = eventProximity.FindVariables(new DatObj(null));
                    VRVariable varObjA = varObjs[0];
                    VRVariable varObjB = varObjs[1];
                    VRVariable varDistance = eventProximity.FindVariable(new DatFloat(0));

                    // Setup Data
                    DatFloat datDistance = (DatFloat)varDistance.vrData;
                    sphereObj.transform.localScale = Vector3.one * datDistance.Value * 2;

                    DatObj datObjA = (DatObj)varObjA.vrData;
                    DatObj datObjB = (DatObj)varObjB.vrData;



                    onFixedUpdate += () =>
                    {
                        // No Dat obj is assigned
                        if (datObjA.Value == null && datObjB.Value == null)
                        {
                            sphereObj.SetActive(false);
                            lineObj.SetActive(false);
                            return;
                        }

                        // At least 1 Dat obj is assigned
                        if (datObjA.Value != null)
                        {
                            if (datObjA.Value.renderer )
                            {
                                sphereObj.transform.position = datObjA.Value.renderer.bounds.center;
                            }
                            else
                            {
                                sphereObj.transform.position = datObjA.Value.gameObject.transform.position;
                            }
                        }
                        else
                        {
                            if (datObjB.Value.renderer )
                            {
                                sphereObj.transform.position = datObjB.Value.renderer.bounds.center;
                            }
                            else
                            {
                                sphereObj.transform.position = datObjB.Value.gameObject.transform.position;
                            }
                        }
                        sphereObj.transform.localScale = Vector3.one * datDistance.Value * 2;
                        sphereObj.SetActive(true);

                        // All Dat obj are assigned
                        if (datObjA.Value != null && datObjB.Value != null)
                        {
                            line.positionCount = 2;
                            if (datObjA.Value.renderer && datObjB.Value.renderer)
                            {
                                line.SetPosition(0, datObjA.Value.renderer.bounds.center);
                                line.SetPosition(1, datObjB.Value.renderer.bounds.center);
                            }
                            else
                            {
                                line.SetPosition(0, datObjA.Value.gameObject.transform.position);
                                line.SetPosition(1, datObjB.Value.gameObject.transform.position);
                            }
                            
                            lineObj.SetActive(true);
                        }
                    };

                    // Remove Debug
                    eventProximity.OnDelete += () => Destroy(sphereObj);
                    eventProximity.OnDelete += () => Destroy(lineObj);
                    break;
            }
        }

        private void OnDestroy()
        {
            if (vrEvent == null)
                return;


        }
    }
}
