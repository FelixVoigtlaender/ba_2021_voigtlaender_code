using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HandRemover : MonoBehaviour
{
    public Toggle toggle;
    [Header("Input")]
    public InputActionReference button;
    fvInputManager inputManager;
    fvInputManager.ButtonHandler handler;
    private void Awake()
    {
        inputManager = GetComponent<fvInputManager>();

        handler = inputManager.FindButtonHandler(button);
        handler.OnButtonDown += OnButtonDown;
    }
    public void OnButtonDown(InputAction.CallbackContext context)
    {
        if (!toggle.isOn)
            return;
        if (!inputManager.currentUIElement)
            return;
        VisObject visObject = inputManager.currentUIElement.GetComponentInParent<VisObject>();
        if (visObject)
        {
            visObject.Delete();
            return;
        }
        VisLogicElement logicElement = inputManager.currentUIElement.GetComponentInParent<VisLogicElement>();
        if (logicElement)
        {
            logicElement.Delete();
        }
    }


}
