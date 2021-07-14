using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionPopulation : MonoBehaviour
{
    public RectTransform content;
    public List<VisAction> visActions;
    void Start()
    {
        PopulateList(content);
    }
    public void PopulateList(RectTransform rect)
    {
        List<VRAction> allVRActions = VRAction.GetAllActions();
        visActions = new List<VisAction>();
        GameObject visActionPrefab = VisManager.instance.GetVisLogicPrefab(allVRActions[0]);


        for (int i = 0; i < allVRActions.Count; i++)
        {
            //Instantiate in List
            VisAction visAction = InstantiateElement(visActionPrefab, i);

            //Setup VisAction with VRAction
            VRAction vrAction = (VRAction)allVRActions[i].CreateInstance();
            vrAction.Setup();
            visAction.Setup(vrAction);

            visActions.Add(visAction);
        }
    }


    public void FixedUpdate()
    {
        for (int i = 0; i < visActions.Count; i++)
        {
            if (visActions[i].GetRootCanvas().transform.parent != content.transform)
            {
                RepopulateList(i);
            }
        }
    }
    public void RepopulateList(int index)
    {
        VRDebug.Log("REPOPULATING");

        List<VRAction> allVRActions = VRAction.GetAllActions();
        GameObject visActionPrefab = VisManager.instance.GetVisLogicPrefab(allVRActions[0]);

        // Copy old Action to new Action
        VisAction currentVisAction = InstantiateElement(visActionPrefab, index);
        VisAction oldVisAction = visActions[index];
        VRAction currentVRAction = (VRAction)oldVisAction.vrAction.CreateInstance();
        currentVRAction.Setup();

        currentVisAction.Setup(currentVRAction);

        oldVisAction.isDeleteAble = true;

        // override old visAction
        visActions[index] = currentVisAction;
    }

    public VisAction InstantiateElement(GameObject prefab, int index)
    {
        GameObject objLogicElement = VisManager.instance.InitPrefabWithCanvas(prefab, content.transform.position);
        VisAction visAction = objLogicElement.GetComponent<VisAction>();
        visAction.GetRootCanvas().transform.SetParent(content.transform);
        visAction.GetRootCanvas().transform.SetSiblingIndex(index);
        visAction.isDeleteAble = false;
        return visAction;
    }
}
