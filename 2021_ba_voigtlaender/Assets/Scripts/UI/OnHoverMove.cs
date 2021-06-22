using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class OnHoverMove : UIOnHoverEvent
{
    // Start is called before the first frame update
    public RectTransform moveTransform;
    public float moveZ = -100f;
    Vector3 cachedPosition;
    void Start()
    {
        if (!moveTransform)
            moveTransform = GetComponent<RectTransform>();

        cachedPosition = moveTransform.anchoredPosition3D;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        lastEntry = Time.time;

        //moveTransform.DOAnchorPos3D(cachedPosition + offset, easeTime);
        moveTransform.DOAnchorPos3DZ(moveZ, easeTime);

        CancelInvoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        lastExit = Time.time;

        Invoke("Close", waitTime);
    }


    public void Close()
    {
        moveTransform.DOAnchorPos3DZ(0, easeTime);
    }
}
