using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownWorkaround : MonoBehaviour
{

    void OnEnable()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 1;
        }
    }
}
