using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Panel : MonoBehaviour
{
    [SerializeField]
    bool isOpen;
    public bool IsOpen
    {
        get 
        { 
            return isOpen; 
        }
        set
        {
            isOpen = value;
            if (isOpen)
                Open();
            else
                Close();
        }
    }


    public float easeTime = 0.1f;
    Vector3 cachedScale;
    CanvasGroup canvasGroup;
    private void Start()
    {
        cachedScale = transform.localScale;
        canvasGroup = GetComponent<CanvasGroup>();
        IsOpen = isOpen;


    }
    public void Open()
    {
        isOpen = true;

        gameObject.SetActive(true);

        //transform.DOScale(cachedScale, easeTime);
        if (canvasGroup)
            canvasGroup.DOFade(1, easeTime);

    }
    public void Close()
    {
        isOpen = false;
        if (canvasGroup)
            canvasGroup.DOFade(0, easeTime).OnComplete(()=>gameObject.SetActive(false));
        //transform.DOScale(0, easeTime);
    }
    public void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }
}
