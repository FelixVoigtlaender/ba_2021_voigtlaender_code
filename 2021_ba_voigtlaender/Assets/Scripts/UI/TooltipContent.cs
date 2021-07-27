using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description = "";

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.instance.Enter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.instance.Exit(this);
    }
}
