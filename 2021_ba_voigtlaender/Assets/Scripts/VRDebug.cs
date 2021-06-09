using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRDebug : MonoBehaviour
{
    static VRDebug instance;
    Text text;
    public void Awake()
    {
        text = GetComponent<Text>();
        instance = this;
    }

    public static void Log(string message)
    {
        //Debug.Log(message);
        if (!instance)
            return;

        instance.text.text += "\n" + message;
    }
    public static void SetLog(string message)
    {
        //Debug.Log(message);
        if (!instance)
            return;

        instance.text.text = message;
    }

}
