using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DropdownWorkaround : MonoBehaviour
{
    void OnEnable()
    {
        if(TryGetComponent(out Canvas canvas))
        {
            canvas.sortingOrder = 5;
        }
    }


}
