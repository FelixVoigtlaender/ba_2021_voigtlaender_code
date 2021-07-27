using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICenter : MonoBehaviour
{
    private static UICenter instance;
    public static UICenter Instance
    { 
        get 
        {
            if (!instance)
                instance = FindObjectOfType<UICenter>();

            return instance;
        }
    }
}
