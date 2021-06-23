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

    public bool isDominant = true;
    TransformGrab otherHand;

    private void Awake()
    {
        inputManager = GetComponent<fvInputManager>();
        draggedTransform = null;

        handler = inputManager.FindButtonHandler(button);
        handler.OnButtonDown += OnButtonDown;
        handler.OnButtonUp += OnButtonUp;

        otherHand = FindOtherHand();
        otherHand.isDominant = false;
    }
    TransformGrab FindOtherHand()
    {
        TransformGrab[] hands = FindObjectsOfType<TransformGrab>();
        foreach(TransformGrab hand in hands)
        {
            if (hand != this)
                return hand;
        }
        return null;
    }

    public void OnButtonDown(InputAction.CallbackContext context)
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;


        if (inputManager.currentUIElement)
        {
            Canvas canvas = inputManager.currentUIElement.GetComponentInParent<Canvas>();
            if (canvas)
                draggedTransform = new DraggedTransform(canvas.transform, transform,inputManager.currentUIHitPosition);
        }
        else if(Physics.Raycast(origin,direction, out RaycastHit hit, maxDistance, layerMask, QueryTriggerInteraction.Collide))
        {
            draggedTransform = new DraggedTransform(hit, transform);
        }

        if(draggedTransform!=null && draggedTransform.transform && draggedTransform.transform.GetComponent<BlockDrag>())
        {
            draggedTransform = null;
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
        // Rotation
        draggedTransform.transform.rotation = transform.rotation * draggedTransform.relativeRotation;
        // Drag
        Vector3 handOffset = (transform.position - draggedTransform.startHandPosition);
        float handDirOffset = Vector3.Dot(transform.forward.normalized, handOffset)*1.1f;
        draggedTransform.goalPosition = transform.position + transform.forward.normalized * (draggedTransform.distance + handDirOffset) - draggedTransform.offset;
        draggedTransform.currentPosition = Vector3.SmoothDamp(draggedTransform.transform.position, draggedTransform.goalPosition, ref draggedTransform.smoothVelocity,0.1f);

        if (draggedTransform.rigid)
            draggedTransform.rigid.velocity = Vector3.zero;

        if (otherHand.draggedTransform!=null && otherHand.draggedTransform.transform == draggedTransform.transform)
        {
            HandleDualWeild();
        }
        else
        {

            draggedTransform.transform.position = draggedTransform.currentPosition;
        }
        
    }

    public void HandleDualWeild()
    {
        DraggedTransform otherTransform = otherHand.draggedTransform;

        Vector3 dif = otherTransform.currentPosition - draggedTransform.currentPosition;
        //TODO
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
        public RectTransform rectTransform;
        public float distance;
        public Vector3 offset;
        public Quaternion relativeRotation;
        public Rigidbody rigid;

        public Vector3 startHandPosition;

        public Vector3 smoothVelocity;

        public Vector3 goalPosition;
        public Vector3 currentPosition;

        public DraggedTransform(RaycastHit hit, Transform handTransform)
        {
            transform = hit.transform;
            distance = hit.distance;
            offset = hit.point - hit.transform.position;
            startHandPosition = handTransform.position;
            rectTransform = transform.GetComponent<RectTransform>();

            relativeRotation = Quaternion.Inverse(handTransform.rotation) * transform.rotation;

            rigid = hit.transform.GetComponent<Rigidbody>();
        }
        public DraggedTransform(Transform transform, Transform handTransform)
        {
            this.transform = transform;
            distance =Vector3.Distance(transform.position,handTransform.position);
            offset = Vector3.zero;
            startHandPosition = handTransform.position;
            rectTransform = transform.GetComponent<RectTransform>();

            relativeRotation = Quaternion.Inverse(handTransform.rotation) * transform.rotation;
        }
        public DraggedTransform(Transform transform, Transform handTransform, Vector3 worldHitPosition)
        {
            this.transform = transform;
            distance = Vector3.Distance(transform.position, handTransform.position);
            offset = worldHitPosition - transform.position;
            startHandPosition = handTransform.position;
            rectTransform = transform.GetComponent<RectTransform>();

            relativeRotation = Quaternion.Inverse(handTransform.rotation) * transform.rotation;
        }
    }
}
