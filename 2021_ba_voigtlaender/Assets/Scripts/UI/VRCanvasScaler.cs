using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCanvasScaler : MonoBehaviour
{
    RectTransform rect;
    public float scaleRatio = 0.0005f;
    public float minScaleRatio = 0.001f;
    public CenterType centerType = CenterType.UICenter;
    private void Awake()
    {
        if (TryGetComponent(out Canvas canvas))
        {
            if (!canvas.worldCamera)
                canvas.worldCamera = Camera.main;
        }
    }

    private void Start()
    {
        rect = GetRectTransform();  
        
    }
    public void Update()
    {
        float distance = GetDistance();
        float scale = Mathf.Max(minScaleRatio, scaleRatio * distance);
        rect.localScale = Vector3.one * (scale);
    }
    public float GetDistance()
    {
        float distance = 1;
        Vector3 myPosition = transform.position;
        Vector3 otherPosition = Vector3.zero;
        myPosition.y = 0;
        switch (centerType)
        {
            case CenterType.UICenter:
                otherPosition = UICenter.Instance.transform.position;
                otherPosition.y = 0;
                distance = (myPosition - otherPosition).magnitude;
                distance += UICenter.Instance.transform.position.z;
                break;
            case CenterType.Camera:
                otherPosition = Camera.main.transform.position;
                otherPosition.y = 0;
                distance = (myPosition - otherPosition).magnitude;
                break;
        }
        return distance;
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
