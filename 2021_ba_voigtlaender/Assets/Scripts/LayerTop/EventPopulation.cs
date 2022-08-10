using System.Collections.Generic;
using LayerBottom;
using UnityEngine;

namespace LayerTop
{
    public class EventPopulation : MonoBehaviour
    {
        public RectTransform content;
        public List<VisEvent> visEvents;
        void Start()
        {
            PopulateList(content);
        }

        public void PopulateList(RectTransform rect)
        {
            List<VREvent> allVREvents = VREvent.GetAllEvents();
            visEvents = new List<VisEvent>();
            GameObject visEventPrefab = VisManager.instance.GetVisLogicPrefab(allVREvents[0]);


            for(int i = 0; i < allVREvents.Count; i++)
            {
                //Instantiate in List
                VisEvent visEvent = InstantiateElement(visEventPrefab, i);

                //Setup VisEvent with VREvent
                VREvent vrEvent =(VREvent) allVREvents[i].CreateInstance();
                vrEvent.Setup();
                visEvent.Setup(vrEvent);
                vrEvent.isRoot = false;

                visEvents.Add(visEvent);
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < visEvents.Count; i++)
            {
                if(visEvents[i].GetRootCanvas().transform.parent != content.transform)
                {
                    RepopulateList(i);
                }
            }
        }

        public void RepopulateList(int index)
        {
            List<VREvent> allVREvents = VREvent.GetAllEvents();
            GameObject visEventPrefab = VisManager.instance.GetVisLogicPrefab(allVREvents[0]);

            // Copy old event to new event
            VisEvent currentVisEvent = InstantiateElement(visEventPrefab, index);
            VisEvent oldVisEvent = visEvents[index];
            VREvent currentVREvent = (VREvent) oldVisEvent.vrEvent.CreateInstance();
            currentVREvent.Setup();

            currentVisEvent.Setup(currentVREvent);
            currentVREvent.isRoot = false;
            oldVisEvent.isDeleteAble = true;
            oldVisEvent.GetElement().isRoot = true;
            oldVisEvent.vrEvent.Save();

            // override old visEvent
            visEvents[index] = currentVisEvent;
        }

        public VisEvent InstantiateElement(GameObject prefab, int index)
        {
            GameObject objLogicElement = VisManager.instance.InitPrefabWithCanvas(prefab, content.transform.position);
            VisEvent visEvent = objLogicElement.GetComponent<VisEvent>();
            visEvent.GetRootCanvas().transform.SetParent(content.transform);
            visEvent.GetRootCanvas().transform.SetSiblingIndex(index);
            visEvent.isDeleteAble = false;
            return visEvent;
        }
    }
}
