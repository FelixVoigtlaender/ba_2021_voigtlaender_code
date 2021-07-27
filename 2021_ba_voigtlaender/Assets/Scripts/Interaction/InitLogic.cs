using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



[RequireComponent(typeof(fvInputManager))]
public class InitLogic : MonoBehaviour
{
    public BetterToggle toggle;
    [Header("Input")]
    public InputActionReference button;
    fvInputManager inputManager;
    fvInputManager.ButtonHandler handler;

    [Header("Raycast")]
    public LayerMask layerMask;
    public float maxDistance;

    private void Awake()
    {
        inputManager = GetComponent<fvInputManager>();

        handler = inputManager.FindButtonHandler(button);
        handler.OnButtonDown += OnButtonDown;
        handler.OnButtonUp += OnButtonUp;
    }

    private void Start()
    {
        
    }
    public void OnButtonDown(InputAction.CallbackContext context)
    {
        if (!toggle.isOn)
            return;

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
