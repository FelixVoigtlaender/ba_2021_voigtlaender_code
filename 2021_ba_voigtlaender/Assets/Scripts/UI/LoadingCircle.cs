using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingCircle : MonoBehaviour
{
    public Image image;
    public Canvas canvas;
    public CanvasGroup canvasGroup;
    public float fillAmount
    {
        get
        {
            if (!image)
                return 1;
            return image.fillAmount;
        }
        set
        {
            if (!image)
                return;
            image.fillAmount = value;
        }
    }
    public float alpha
    {
        get
        {
            if (!canvasGroup)
                return 1;
            return canvasGroup.alpha;
        }
        set
        {
            if (!canvasGroup)
                return;
            canvasGroup.alpha = value;
        }
    }
}
