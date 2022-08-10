using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRCanvasScaler : MonoBehaviour
{
    RectTransform rect;
    public float scaleRatio = 0.0005f;
    public float minScaleRatio = 0.001f;

    public bool useDistance = true;
    public CenterType centerType = CenterType.UICenter;

    public bool useXRRigScale = false;
    XRRig xrRig;
    private void Awake()
    {
        xrRig = FindObjectOfType<XRRig>();
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

        float scale = minScaleRatio;
        if(useDistance)
            scale = Mathf.Max(minScaleRatio, scaleRatio * distance / xrRig.transform.localScale.x);
        if (useXRRigScale)
            scale *= xrRig.transform.localScale.x;
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
