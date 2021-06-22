using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class OnHoverScale : UIOnHoverEvent
{

    public Transform scaleTransform;
    Vector3 cachedScale;
    void Start()
    {
        cachedScale = scaleTransform.localScale;
        scaleTransform.localScale = Vector3.zero;
        OnPointerEnter(null);
    }


    public override void OnPointerEnter(PointerEventData eventData)
    {
        lastEntry = Time.time;
        scaleTransform.DOScale(1, easeTime);

        CancelInvoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        lastExit = Time.time;

        Invoke("Close", 5);
    }


    public void Close()
    {
        scaleTransform.DOScale(0, easeTime);
    }
}
