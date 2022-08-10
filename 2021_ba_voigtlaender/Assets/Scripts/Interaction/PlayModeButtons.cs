using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayModeButtons : MonoBehaviour
{
    public List<ButtonAnnotation> buttonAnnotations = new List<ButtonAnnotation>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (var bA in buttonAnnotations)
        {
            fvInputModeManager.instance.AddButtonMode(bA.button, bA.buttonText, bA.modeName);
        }
    }


    [Serializable]
    public class ButtonAnnotation
    {
        public InputActionReference button;
        public string modeName = "";
        public string buttonText = "";
    }
}
