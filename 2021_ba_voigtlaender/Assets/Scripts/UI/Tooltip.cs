using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tooltip : MonoBehaviour
{
    public static Tooltip instance;

    TooltipContent activeContent;

    public Text textField;
    public RectTransform background;
    Canvas canvas;
    private void Awake()
    {
        instance = this;
        canvas = GetComponentInParent<Canvas>();
    }


    public void Enter(TooltipContent content)
    {
        if (content == activeContent)
            return;

        background.transform.localScale = Vector3.zero;
        background.DOAnchorPos3DZ(-20, 0.1f);
        background.DOScale(1, 0.1f);

        activeContent = content;

        canvas.transform.position = content.transform.position - content.transform.forward*0.05f;
        canvas.transform.rotation = content.transform.rotation;
        textField.text = content.description;
        canvas.gameObject.SetActive(true);
    }

    public void Exit(TooltipContent content)
    {
        if (content != activeContent)
            return;

        background.DOAnchorPos3DZ(0, 0.1f);
        background.DOScale(0, 0.1f);

        activeContent = null;
    }
}
