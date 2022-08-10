using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DropdownWorkaround : MonoBehaviour
{
    public int sortingLayer = 5;
    void OnEnable()
    {
        if(TryGetComponent(out Canvas canvas))
        {
            canvas.sortingOrder = sortingLayer;
        }
    }


}
