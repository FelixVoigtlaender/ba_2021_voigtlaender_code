using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIOnHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public float easeTime = .1f;
    public float waitTime = 5f;
    protected float lastEntry;
    protected float lastExit;



    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        lastEntry = Time.time;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        lastExit = Time.time;
    }

}