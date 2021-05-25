using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCanvasScaler : MonoBehaviour
{
    RectTransform rect;
    public float scaleRatio = 0.01f;
    private void Start()
    {
        rect = GetRectTransform();   
    }
    public void Update()
    {
        float distance = (transform.position - Camera.main.transform.position).magnitude;
        rect.localScale = Vector3.one * (scaleRatio * distance);
    }
    private void OnValidate()
    {
        GetRectTransform().localScale = Vector3.one * (scaleRatio);
    }

    public RectTransform GetRectTransform()
    {
        if (!rect)
            rect = GetComponent<RectTransform>();
        return rect;
    }
}
