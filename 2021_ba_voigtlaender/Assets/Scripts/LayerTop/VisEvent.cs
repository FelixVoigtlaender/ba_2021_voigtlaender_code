using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisEvent : VisLogicElement
{
    public VREvent vrEvent;
    public string eventName;
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

    public void SetupTypes(VREvent vrEvent)
    {
        switch (vrEvent)
        {
            case EventProximity eventProximity:
                // Setup Debug
                GameObject spherePrefab = VisManager.instance.prefabDebugSphere;
                GameObject sphere = Instantiate(spherePrefab);
                sphere.SetActive(false);

                //Setup Variables
                VRVariable varObj = eventProximity.FindVariable(new DatObj(null));
                VRVariable varDistance = eventProximity.FindVariable(new DatFloat(0));

                // Setup Data
                DatFloat datDistance = (DatFloat)varDistance.vrData;
                sphere.transform.localScale = Vector3.one * datDistance.Value * 2;
                datDistance.OnDataChanged += (value) => 
                {
                    sphere.transform.localScale = Vector3.one * datDistance.Value * 2;
                };

                DatObj datObj = (DatObj)varObj.vrData;
                datObj.OnDataChanged += (value) =>
                {
                    if (datObj.Value == null)
                    {
                        sphere.SetActive(false);
                    }
                    else
                    {
                        sphere.transform.position = datObj.Value.gameObject.transform.position;
                        sphere.SetActive(true);
                    }
                };

                // Remove Debug
                eventProximity.OnDelete += () => Destroy(sphere);
                break;
        }
    }
}
