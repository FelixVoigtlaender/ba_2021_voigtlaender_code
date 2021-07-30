using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(fvInputManager))]
public class MoveGrab : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference button;
    public string modeName = "";
    public string buttonText = "";
    fvInputManager inputManager;

    fvInputModeManager inputModeManager;
    fvInputModeManager.ButtonModeHandler handler;

    [Header("Grabbing")]
    public Transform xrRig;
    public Vector3 worldGrabPos;
    public Vector3 localGrabPos;
    public bool isGrabbing;
    public bool isDominant;
    MoveGrab otherHand;

    GameObject helperObject;
    private void Start()
    {
        inputManager = GetComponentInParent<fvInputManager>();
        inputModeManager = GetComponentInParent<fvInputModeManager>();

        handler = inputModeManager.AddButtonMode(button, buttonText, modeName);
        handler.OnButtonDown += OnButtonDown;
        handler.OnButtonUp += OnButtonUp;

        helperObject = new GameObject("HelperObject");
        helperObject.transform.localScale = Vector3.one;

        otherHand = FindOtherHand();
        if (isDominant)
            otherHand.isDominant = false;
    }
    MoveGrab FindOtherHand()
    {
        MoveGrab[] hands = FindObjectsOfType<MoveGrab>();
        foreach (MoveGrab hand in hands)
        {
            if (hand != this)
                return hand;
        }
        return null;
    }

    public void OnButtonDown(InputAction.CallbackContext context)
    {
        isGrabbing = true;


        worldGrabPos = transform.position;
        localGrabPos = transform.localPosition;
    }

    public void Update()
    {
        if (!isGrabbing)
            return;

        HandleGrabbing();
    }

    public void HandleGrabbing()
    {
        if (!isGrabbing)
            return;
        if (otherHand.isGrabbing)
            HandleDualWeild();
        else
            HandleSingleWeild();
    }

    public void HandleSingleWeild()
    {

        Vector3 currentWorldGrabPos = transform.position;
        Vector3 worldGrabPos = this.worldGrabPos;

        Vector3 difference = currentWorldGrabPos - worldGrabPos;

        xrRig.transform.position -= difference;
    }
    public void HandleDualWeild()
    {
        if (!isDominant)
            return;
        

        Vector3 otherInitialWorld = otherHand.worldGrabPos;
        Vector3 otherCurrentWorld = otherHand.transform.position;

        Vector3 initialWorld = worldGrabPos;
        Vector3 currentWorld = transform.position;

        Vector3 initialVector = otherInitialWorld - initialWorld;
        Vector3 currentVector = otherCurrentWorld - currentWorld;

        Vector3 initialMid = initialWorld + initialVector * 0.5f;
        Vector3 currentMid = currentWorld + currentVector * 0.5f;
        Vector3 currentMidLocal = xrRig.InverseTransformPoint(currentMid);

        Debug.DrawRay(initialWorld, initialVector, Color.red);
        Debug.DrawRay(initialMid, Vector3.up * 0.01f, Color.red);

        Debug.DrawRay(currentWorld, currentVector, Color.green);
        Debug.DrawRay(currentMid, Vector3.up * 0.01f, Color.green);



        // Rotation
        Vector3 initialVectorFlat = new Vector3(initialVector.x, 0, initialVector.z);
        Vector3 currentVectorFlat = new Vector3(currentVector.x, 0, currentVector.z);
        Vector3 axis = Vector3.Cross(initialVectorFlat.normalized, currentVectorFlat.normalized);
        xrRig.Rotate(axis, -Vector3.Angle(initialVectorFlat.normalized, currentVectorFlat.normalized));

        // Scaling
        if(currentVector.magnitude < 100 && currentVector.magnitude > 0.1f)
        {
            Vector3 localScale = xrRig.transform.localScale;
            localScale *= initialVector.magnitude / currentVector.magnitude;
            localScale = Mathf.Clamp(localScale.x, 1f, 100) * Vector3.one;
            xrRig.transform.localScale = localScale;
            VRDebug.SetLog(xrRig.transform.localScale.ToString());
        }


        // Movement
        Vector3 dif = initialMid - xrRig.TransformPoint(currentMidLocal);
        Debug.DrawRay(xrRig.TransformPoint(currentMidLocal), Vector3.up * 0.01f);
        Debug.DrawRay(initialMid, dif);
        xrRig.transform.position += dif;
    }


    public void HandleFirstIterationGrab()
    {

        if (!otherHand.isGrabbing && !isDominant)
        {
            this.worldGrabPos = transform.position;
            isDominant = true;
        }
        if (!isDominant)
            return;

        Vector3 currentWorldGrabPos = transform.position;
        Vector3 worldGrabPos = this.worldGrabPos;

        Vector3 difference = currentWorldGrabPos - worldGrabPos;

        xrRig.transform.position -= new Vector3(difference.x, 0, difference.z);
        xrRig.transform.localScale -= Vector3.one * difference.y;

        xrRig.transform.localScale = Mathf.Clamp(xrRig.transform.localScale.x, 1, 10) * Vector3.one;
    }


    public void OnButtonUp(InputAction.CallbackContext context)
    {
        isGrabbing = false;
    }
}
