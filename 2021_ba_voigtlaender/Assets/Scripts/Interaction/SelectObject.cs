using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;


[RequireComponent(typeof(fvInputManager))]

public class SelectObject : MonoBehaviour
{

    [Header("Input")]
    public InputActionReference button;
    public string modeName = "";
    public string buttonText = "";
    fvInputManager inputManager;

    public event Action<GameObject> OnSelectedObject;

    fvInputModeManager inputModeManager;
    fvInputModeManager.ButtonModeHandler handler;

    private void Start()
    {

        inputManager = GetComponentInParent<fvInputManager>();
        inputModeManager = GetComponentInParent<fvInputModeManager>();

        handler = inputModeManager.AddButtonMode(button, buttonText, modeName);
        handler.OnButtonDown += OnButtonDown;
        handler.OnButtonUp += OnButtonUp;
    }

    public void OnButtonDown(InputAction.CallbackContext context)
    {
        GameObject selectedObject = Select();
        OnSelectedObject?.Invoke(selectedObject);
    }
    public void OnButtonUp(InputAction.CallbackContext context)
    {
    }

    public GameObject Select()
    {

        GameObject logicObject = null;

        if (inputManager.isUIHitClosest)
        {
            if (inputManager.uiRaycastHit.HasValue)
                logicObject = inputManager.uiRaycastHit.Value.gameObject;
        }
        else
        {
            if (inputManager.worldRaycastHit.HasValue)
                logicObject = inputManager.worldRaycastHit.Value.collider.gameObject;
        }

        if (logicObject == null)
            return null;
        if (logicObject.GetComponentInParent<BlockCode>())
            return null;

        return logicObject;
    }
}
