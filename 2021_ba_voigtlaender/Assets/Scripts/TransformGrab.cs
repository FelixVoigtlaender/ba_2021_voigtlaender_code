using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(fvInputManager))]
public class TransformGrab : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference button;
    fvInputManager inputManager;
    fvInputManager.ButtonHandler handler;

    [Header("Dragging")]
    public LayerMask layerMask;
    public DraggedTransform draggedTransform;
    public float maxDistance = 30;
    public float pullSpeed = 1;
    public float scaleSpeed = 1;

    private void Awake()
    {
        inputManager = GetComponent<fvInputManager>();
        draggedTransform = null;

        handler = inputManager.FindButtonHandler(button);
        handler.OnButtonDown += OnButtonDown;
        handler.OnButtonUp += OnButtonUp;
    }

    public void OnButtonDown(InputAction.CallbackContext context)
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;


        if (inputManager.currentUIElement)
        {
            Canvas canvas = inputManager.currentUIElement.GetComponentInParent<Canvas>();
            if (canvas)
                draggedTransform = new DraggedTransform(canvas.transform, transform);
        }
        else if(Physics.Raycast(origin,direction, out RaycastHit hit, maxDistance, layerMask, QueryTriggerInteraction.Collide))
        {
            draggedTransform = new DraggedTransform(hit, transform);
        }
    }
    public void OnButtonUp(InputAction.CallbackContext context)
    {
        draggedTransform = null;
    }
    public void Update()
    {
        HandleDrag();
        HandlePull();
        HandleScale();
    }
    public void HandleDrag()
    {
        if (!handler.isPressed)
            draggedTransform = null;
        if (draggedTransform == null ||!draggedTransform.transform)
            return;


        VRDebug.SetLog("CurrentDrag: " + draggedTransform.transform.name);
        // Drag
        Vector3 position = transform.position + transform.forward.normalized * draggedTransform.distance - draggedTransform.offset;
        draggedTransform.transform.position = position;
        draggedTransform.transform.rotation = transform.rotation * draggedTransform.relativeRotation;

        if (draggedTransform.rigid)
            draggedTransform.rigid.velocity = Vector3.zero;
    }
    public void HandlePull()
    {
        if (!handler.isPressed)
            draggedTransform = null;
        if (draggedTransform == null || !draggedTransform.transform)
            return;

        float input = inputManager.joystickDir.y;
        if (Mathf.Abs(input) < 0.2f)
            return;

        draggedTransform.distance += input * pullSpeed * Time.deltaTime;
        draggedTransform.distance = Mathf.Clamp(draggedTransform.distance, 0, maxDistance);
    }
    public void HandleScale()
    {
        if (!handler.isPressed)
            draggedTransform = null;
        if (draggedTransform == null || !draggedTransform.transform)
            return;

        float input = inputManager.joystickDir.x;
        if (Mathf.Abs(input) < 0.2f)
            return;

        Vector3 localScale = draggedTransform.transform.localScale;
        localScale += input * Vector3.one * scaleSpeed * Time.deltaTime;
        localScale.x = Mathf.Clamp(localScale.x, 0, 100);
        localScale.y = Mathf.Clamp(localScale.y, 0, 100);
        localScale.z = Mathf.Clamp(localScale.z, 0, 100);
        draggedTransform.transform.localScale = localScale;
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
        public Quaternion relativeRotation;
        public Rigidbody rigid;

        public DraggedTransform(RaycastHit hit, Transform handTransform)
        {
            transform = hit.transform;
            distance = hit.distance;
            offset = hit.point - hit.transform.position;

            relativeRotation = Quaternion.Inverse(handTransform.rotation) * transform.rotation;

            rigid = hit.transform.GetComponent<Rigidbody>();
        }
        public DraggedTransform(Transform transform, Transform handTransform)
        {
            this.transform = transform;
            distance =Vector3.Distance(transform.position,handTransform.position);
            offset = Vector3.zero;

            relativeRotation = Quaternion.Inverse(handTransform.rotation) * transform.rotation;
        }
    }
}
