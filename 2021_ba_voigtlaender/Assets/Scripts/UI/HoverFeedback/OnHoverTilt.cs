using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class OnHoverTilt : UIOnHoverEvent
{
    public Transform tiltTransform;
    public RectTransform rectTransform;
    public Vector3 rotation;
    Vector3 cachedRotation;
    void Start()
    {
        cachedRotation = tiltTransform.localEulerAngles;

        if (!tiltTransform)
            tiltTransform = transform;
    }


    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        rectTransform.DOLocalRotate(rotation, easeTime);
        VRDebug.Log("E N T E R");

        CancelInvoke();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        Invoke("Revert", waitTime);
    }

    public void Revert()
    {
        rectTransform.DOLocalRotate(cachedRotation, easeTime);
    }
}
