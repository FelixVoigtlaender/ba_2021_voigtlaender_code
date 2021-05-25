using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(fvInputManager))]
public class TransformGrab : MonoBehaviour
{
    [Header("Input")]
    public InputHelpers.Button button = InputHelpers.Button.None;
    fvInputManager inputManager;
    public fvInputManager.ButtonHandler handler;

    [Header("Dragging")]
    public LayerMask layerMask;
    public DraggedTransform draggedTransform;
    public float maxDistance = 30;

    private void Awake()
    {
        inputManager = GetComponent<fvInputManager>();
        draggedTransform = null;

        handler = inputManager.FindButtonHandler(button);
        handler.OnButtonDown += OnButtonDown;
        handler.OnButtonUp += OnButtonUp;
    }

    public void OnButtonDown(XRController controller)
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if(Physics.Raycast(origin,direction, out RaycastHit hit, maxDistance, layerMask, QueryTriggerInteraction.Collide))
        {
            draggedTransform = new DraggedTransform(hit);
        }
    }
    public void OnButtonUp(XRController controller)
    {
        draggedTransform = null;
    }
    public void Update()
    {
        HandleDrag();
    }
    public void HandleDrag()
    {
        if (!handler.pressed)
            draggedTransform = null;
        if (draggedTransform == null ||!draggedTransform.transform)
            return;

        Vector3 position = transform.position + transform.forward.normalized * draggedTransform.distance - draggedTransform.offset;
        draggedTransform.transform.position = position;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * 100);
    }

    [System.Serializable]
    public class DraggedTransform
    {
        public Transform transform;
        public float distance;
        public Vector3 offset;

        public DraggedTransform(RaycastHit hit)
        {
            transform = hit.transform;
            distance = hit.distance;
            offset = hit.point - hit.transform.position;
        }
    }
}
