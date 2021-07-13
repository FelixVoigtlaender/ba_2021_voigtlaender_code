using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentPopulation : MonoBehaviour
{
    public RectTransform content;
    public GameObject prefab;
    public int amount = 10;
    // Start is called before the first frame update
    void Start()
    {
        PopulateList(content, amount);
    }

    public void PopulateList(RectTransform rect, int amount)
    {

        for(int i = 0; i < amount; i++)
        {
            GameObject objLogicElement = VisManager.instance.InitPrefabWithCanvas(prefab, rect.transform.position);
            Transform root = objLogicElement.transform.root;
            root.transform.SetParent(rect.transform);

            VisLogicElement visLogicElement = objLogicElement.GetComponent<VisLogicElement>();
            visLogicElement.Init();
        }
    }
}
