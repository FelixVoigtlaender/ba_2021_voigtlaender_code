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

    public void OnButtonDown(InputAction.CallbackContext context)
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.GetComponent<BlockCode>())
                return;
            VRManager.instance.InitVRObject(hit.collider.gameObject);
        }
    }
    public void OnButtonUp(InputAction.CallbackContext context)
    {
    }

}
