using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[RequireComponent(typeof(fvInputManager))]
public class TransformGrab : MonoBehaviour
{
    public BetterToggle toggle;
    [Header("Input")]
    public InputActionReference button;
    fvInputManager inputManager;
    fvInputManager.ButtonHandler handler;

    [Header("Dragging")]
    public LayerMask layerMask;
    public GrabbedObject grabbedObject;
    public float maxDistance = 30;
    public float pullSpeed = 1;
    public float scaleSpeed = 1;

    public bool isDominant = true;
    TransformGrab otherHand;

    private void Awake()
    {
        inputManager = GetComponent<fvInputManager>();
        grabbedObject = null;

        handler = inputManager.FindButtonHandler(button);
        handler.OnButtonDown += OnButtonDown;
        handler.OnButtonUp += OnButtonUp;

        otherHand = FindOtherHand();
        if(isDominant)
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
        if (!toggle.isOn)
            return;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward * inputManager.relativeRayLength;




        if (inputManager.isUIHitClosest)
        {
            if (!inputManager.uiRaycastHit.HasValue)
                return;

            UnityEngine.EventSystems.RaycastResult result = inputManager.uiRaycastHit.Value;

            Canvas canvas = result.gameObject.GetComponentInParent<Canvas>();
            if (canvas.GetComponent<BlockDrag>())
                return;

            if (canvas)
            {
                grabbedObject = new GrabbedObject(canvas.transform, transform, result.worldPosition);
                Vector3 worldPosition = canvas.transform.position;
                canvas.transform.SetParent(null, false);
                canvas.transform.position = worldPosition;
            }
        }
        else if(inputManager.worldRaycastHit.HasValue)
        {
            grabbedObject = new GrabbedObject(inputManager.worldRaycastHit.Value, transform);
        }

        if(grabbedObject!=null && grabbedObject.transform && grabbedObject.transform.GetComponent<BlockDrag>())
        {
            grabbedObject = null;
        }
    }


    public void OnButtonUp(InputAction.CallbackContext context)
    {
        if (grabbedObject!=null)
            grabbedObject.Release();
        grabbedObject = null;
    }


    public void Update()
    {
        if (!toggle.isOn)
            return;
        HandleDrag();
        HandlePull();
        HandleScale();
    }


    public void HandleDrag()
    {
        if (!handler.isPressed)
            grabbedObject = null;
        if (grabbedObject == null ||!grabbedObject.transform)
            return;
        if (grabbedObject.blockDrag)
            return;


        if (grabbedObject.rigid)
        {
            grabbedObject.rigid.velocity = Vector3.zero;
            grabbedObject.rigid.angularVelocity = Vector3.zero;
        }


        if (otherHand.grabbedObject!=null && otherHand.grabbedObject.transform == grabbedObject.transform)
        {
            grabbedObject.wasKinematic = otherHand.grabbedObject.wasKinematic = grabbedObject.wasKinematic && otherHand.grabbedObject.wasKinematic;
            // Dual Weild
            HandleDualWeild();
        }
        else
        {
            HandleSingleWeild();
        }
        
    }

    public void CalculatePoints()
    {
        // Get current hit point
        grabbedObject.currentPoint = grabbedObject.transform.InverseTransformPoint(transform.position + transform.forward.normalized * grabbedObject.initialDistance);

        // Smooth point
        //grabbedObject.smoothedPoint = Vector3.SmoothDamp(grabbedObject.currentPoint, grabbedObject.initialPoint, ref grabbedObject.currentSmoothing,0.1f);
        grabbedObject.smoothedPoint = grabbedObject.initialPoint + (grabbedObject.currentPoint - grabbedObject.initialPoint) * 0.2f;
    }

    public void HandleSingleWeild()
    {

        // Single Weild
        VRDebug.SetLog("SINGLE WEILD");

        // Calculate Points
        CalculatePoints();


        // Set Rotation
        if (!grabbedObject.blockRotate)
        {
            grabbedObject.transform.rotation = transform.rotation * grabbedObject.initialRotation;
        }

        // Set position
        if (!grabbedObject.blockDrag)
            grabbedObject.transform.position += grabbedObject.transform.TransformVector(grabbedObject.smoothedPoint - grabbedObject.initialPoint);
    }

    public void HandleDualWeild()
    {
        if (!isDominant)
            return;

        VRDebug.SetLog("DUAL WEILD");
        GrabbedObject o_grabbedObject = otherHand.grabbedObject;
        Transform grabbedTransform = grabbedObject.transform;

        CalculatePoints();
        otherHand.CalculatePoints();

        // Initial Grab state
        Vector3 initialVector = o_grabbedObject.initialPoint - grabbedObject.initialPoint;
        Vector3 initialMiddle = grabbedObject.initialPoint + initialVector * 0.5f;
        Debug.DrawRay(grabbedTransform.TransformPoint(grabbedObject.initialPoint), grabbedTransform.TransformVector(initialVector));

        // Current grab Vector
        Vector3 currentVector = o_grabbedObject.smoothedPoint - grabbedObject.smoothedPoint;
        Vector3 currentMiddle = grabbedObject.smoothedPoint + currentVector * 0.5f;
        Debug.DrawRay(grabbedTransform.TransformPoint(grabbedObject.smoothedPoint), grabbedTransform.TransformVector(currentVector));
        //TODO

        //Rotation
        Vector3 axis = Vector3.Cross(initialVector.normalized, currentVector.normalized);

        if (!grabbedObject.rectTransform && !grabbedObject.blockRotate)
        {
            grabbedTransform.Rotate(axis, Vector3.Angle(initialVector, currentVector));
            grabbedObject.initialRotation = Quaternion.Inverse(transform.rotation) * grabbedTransform.rotation;
            o_grabbedObject.initialRotation = Quaternion.Inverse(otherHand.transform.rotation) * grabbedTransform.rotation;
        }

        //Position
        Vector2 deltaMid = currentMiddle - initialMiddle;
        grabbedTransform.position += grabbedTransform.TransformPoint(currentMiddle) - grabbedTransform.TransformPoint(initialMiddle);

        //Scaling
        if (!grabbedObject.rectTransform)
        {
            Vector3 scale = grabbedTransform.localScale;
            scale *= currentVector.magnitude / initialVector.magnitude;
            grabbedTransform.localScale = scale;
        }
        else
        {
            RectTransform rectTransform = grabbedObject.rectTransform;
            VRDebug.SetLog(rectTransform.sizeDelta.ToString());
        }
    }


    public void HandlePull()
    {
        if (!handler.isPressed)
            grabbedObject = null;
        if (grabbedObject == null || !grabbedObject.transform)
            return;

        return;

        float input = inputManager.joystickDir.y;
        if (Mathf.Abs(input) < 0.2f)
            return;

        grabbedObject.initialDistance += input * pullSpeed * Time.deltaTime;
        grabbedObject.initialDistance = Mathf.Clamp(grabbedObject.initialDistance, 0, maxDistance);
    }

    
    public void HandleScale()
    {
        if (!handler.isPressed)
            grabbedObject = null;
        if (grabbedObject == null || !grabbedObject.transform)
            return;

        float input = inputManager.joystickDir.x;
        if (Mathf.Abs(input) < 0.2f)
            return;

        Vector3 localScale = grabbedObject.transform.localScale;
        localScale += input * Vector3.one * scaleSpeed * Time.deltaTime;
        localScale.x = Mathf.Clamp(localScale.x, 0, 100);
        localScale.y = Mathf.Clamp(localScale.y, 0, 100);
        localScale.z = Mathf.Clamp(localScale.z, 0, 100);
        grabbedObject.transform.localScale = localScale;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * 100);
    }

    [System.Serializable]
    public class GrabbedObject
    {
        public Transform transform;
        public RectTransform rectTransform;
        public Rigidbody rigid;
        public bool wasKinematic;

        // Initial State
        public float initialDistance;
        public Vector3 initialPoint;
        public Quaternion initialRotation;
        public Vector3 initialHandPosition;

        //Blocker
        public BlockCode blockCode;
        public BlockDrag blockDrag;
        public BlockRotate blockRotate;
        

        // Smoothed State
        public Vector3 currentSmoothing;
        public Vector3 smoothedPoint;

        // Current State
        public Vector3 currentPoint;

        // For Transforms with Collider
        public GrabbedObject(RaycastHit hit, Transform handTransform)
        {
            transform = hit.transform;
            initialDistance = hit.distance;

            initialPoint = currentPoint = hit.transform.InverseTransformPoint(hit.point);
            //offset = hit.point - hit.transform.position;



            initialHandPosition = handTransform.position;
            rectTransform = transform.GetComponent<RectTransform>();

            initialRotation = Quaternion.Inverse(handTransform.rotation) * transform.rotation;

            if (hit.transform.TryGetComponent(out rigid))
            {
                wasKinematic = rigid.isKinematic;
                rigid.isKinematic = true;
            }

            GetBlocker();
        }
        // For Transforms with UI
        public GrabbedObject(Transform transform, Transform handTransform, Vector3 worldHitPosition)
        {
            this.transform = transform;
            initialDistance = Vector3.Distance(transform.position, handTransform.position);
            initialPoint = transform.InverseTransformPoint(worldHitPosition);
            initialHandPosition = handTransform.position;
            rectTransform = transform.GetComponent<RectTransform>();

            initialRotation = Quaternion.Inverse(handTransform.rotation) * transform.rotation;

            GetBlocker();
        }

        void GetBlocker()
        {
            blockCode = transform.GetComponent<BlockCode>();
            blockDrag = transform.GetComponent<BlockDrag>();
            blockRotate = transform.GetComponent<BlockRotate>();
        }

        public void Release()
        {
            if (rigid)
                rigid.isKinematic = wasKinematic;
        }
    }
}
