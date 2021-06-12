using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HandSpawner : MonoBehaviour
{
    public Transform spawnTransform;

    public List<ButtonSpawn> buttonSpawns;

    Transform currentElement;

    private void Start()
    {
        foreach(ButtonSpawn buttonSpawn in buttonSpawns)
        {
            buttonSpawn.button.onClick.AddListener(() => OnButtonClick(buttonSpawn));
        }
    }

    private void Update()
    {
        if (!currentElement)
            return;

        if(currentElement.transform.localPosition.magnitude > 0.05f)
        {
            currentElement.transform.SetParent(null);
            currentElement = null;
        }
    }

    void OnButtonClick(ButtonSpawn buttonSpawn)
    {

        GameObject objLogicElement = VisManager.instance.InitPrefabWithCanvas(buttonSpawn.prefab, spawnTransform.position);
        Transform root = objLogicElement.transform.root;
        root.SetParent(spawnTransform);
        root.localPosition = Vector3.zero;
        currentElement = root;


        VisLogicElement visLogicElement = objLogicElement.GetComponent<VisLogicElement>();
        visLogicElement.Init();
    }



    [System.Serializable]
    public class ButtonSpawn
    {
        public Button button;
        public GameObject prefab;
    }
}
