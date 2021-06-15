using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIOnHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform scaleTransform;
    Vector3 cachedScale;

    void Start()
    {

        cachedScale = scaleTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        VRDebug.Log("Pointer Enter");
        //scaleTransform.localScale = cachedScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        VRDebug.Log("Pointer Exit");
        //scaleTransform.localScale = new Vector3(0, 0, 0);
    }
}