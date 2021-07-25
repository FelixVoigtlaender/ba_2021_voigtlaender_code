using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class OnHoverLine : UIOnHoverEvent
{
    public float inactiveAlpha = 0;
    public float activeAlpha = 1;
    public LineRenderer linerenderer;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        lastEntry = Time.time;


        Color2 startColor = new Color2(linerenderer.startColor, linerenderer.endColor);
        Color2 endColor = new Color2(linerenderer.startColor, linerenderer.endColor);
        endColor.ca.a = activeAlpha;
        endColor.cb.a = activeAlpha;
        linerenderer.DOColor(startColor, endColor, easeTime);


        CancelInvoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        lastExit = Time.time;

        Invoke("Revert", waitTime);
    }


    public void Revert()
    {
        Color2 startColor = new Color2(linerenderer.startColor,linerenderer.endColor);
        Color2 endColor = new Color2(linerenderer.startColor, linerenderer.endColor);
        endColor.ca.a = inactiveAlpha;
        endColor.cb.a = inactiveAlpha;
        linerenderer.DOColor(startColor, endColor, easeTime);
    }
}
