using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



[RequireComponent(typeof(fvInputManager))]
public class InitLogic : MonoBehaviour
{
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
        if (!this.enabled)
            return;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (inputManager.currentUIElement)
        {
            if (inputManager.currentUIElement.GetComponentInParent<BlockCode>())
                return;
            VRManager.instance.InitVRObject(inputManager.currentUIElement);
            return;
        }

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.GetComponentInParent<BlockCode>())
                return;
            VRManager.instance.InitVRObject(hit.collider.gameObject);
        }
    }
    public void OnButtonUp(InputAction.CallbackContext context)
    {
    }

}
