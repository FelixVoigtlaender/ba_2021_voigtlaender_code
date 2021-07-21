using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(fvInputManager))]
public class MoveGrab : MonoBehaviour
{
    public Toggle toggle;

    [Header("Input")]
    public InputActionReference button;
    fvInputManager inputManager;
    fvInputManager.ButtonHandler handler;

    [Header("Grabbing")]
    public Transform xrRig;
    public Vector3 worldGrabPos;
    public bool isGrabbing;
    public bool isDominant;
    MoveGrab otherHand;
    private void Awake()
    {
        inputManager = GetComponent<fvInputManager>();

        handler = inputManager.FindButtonHandler(button);
        handler.OnButtonDown += OnButtonDown;
        handler.OnButtonUp += OnButtonUp;

        otherHand = FindOtherHand();
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
        if (!toggle.isOn)
            return;
        isGrabbing = true;
    }

    public void Update()
    {
        if (!toggle.isOn)
            return;
        if (!isGrabbing)
            return;
        if (!otherHand.isGrabbing && !isDominant)
        {
            this.worldGrabPos = transform.position;
            isDominant = true;
        }
        if (!isDominant)
            return;

        Vector3 currentWorldGrabPos = transform.position;
        Vector3 worldGrabPos = this.worldGrabPos;

        Vector3 difference =  currentWorldGrabPos - worldGrabPos;

        xrRig.transform.position -= new Vector3(difference.x, 0, difference.z);
        xrRig.transform.localScale -= Vector3.one * difference.y;

        xrRig.transform.localScale = Mathf.Clamp(xrRig.transform.localScale.x, 1, 10) * Vector3.one;
    }


    public void OnButtonUp(InputAction.CallbackContext context)
    {
        if (!toggle.isOn)
            return;
        isGrabbing = false;
        isDominant = false;
    }
}
