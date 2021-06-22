using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandRemover : MonoBehaviour
{
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
        //VRDebug.Log("Delete Pressed");
        
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
