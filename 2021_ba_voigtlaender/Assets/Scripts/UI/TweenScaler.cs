using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TweenScaler : MonoBehaviour
{
    public Transform target;
    public float easeTime = 0.1f;
    

    public void Tween(float size)
    {
        target.DOScale(size, easeTime);
    }

}
