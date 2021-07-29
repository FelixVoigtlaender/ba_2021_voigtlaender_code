using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



[RequireComponent(typeof(fvInputManager))]
public class InitLogic : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference button;
    public string modeName = "";
    public string buttonText = "";
    fvInputManager inputManager;

    fvInputModeManager inputModeManager;
    fvInputModeManager.ButtonModeHandler handler;

    [Header("Raycast")]
    public LayerMask layerMask;
    public float maxDistance;

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
            return;
        if (logicObject.GetComponentInParent<BlockCode>())
            return;


        VRManager.instance.InitVRObject(logicObject);
    }
    public void OnButtonUp(InputAction.CallbackContext context)
    {
    }

}
