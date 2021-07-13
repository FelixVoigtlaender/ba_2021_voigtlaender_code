using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class OnHoverShake : UIOnHoverEvent
{
    public Transform shakeTransform;
    public float strength = 0.01f;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        Shake();
    }

    public void Shake()
    {
        if (Time.time - lastEntry < waitTime)
            return;


        shakeTransform.DOShakePosition(easeTime, strength);

        lastEntry = Time.time;
    }
}
