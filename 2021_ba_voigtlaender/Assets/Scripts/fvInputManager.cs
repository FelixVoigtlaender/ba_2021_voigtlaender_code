using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class fvInputManager : MonoBehaviour
{
    public List<ButtonHandler> allButtonHandlers = new List<ButtonHandler>();
    private XRRayInteractor rayInteractor;

    public Vector2 joystickDir;
    public Vector3 relativeJoystickDir;

    public InputActionReference joystick;

    public GameObject currentUIElement;
    private void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();

        rayInteractor.hoverEntered.AddListener(HoverEntered);
        rayInteractor.hoverExited.AddListener(HoverExited);
        rayInteractor.selectEntered.AddListener(SelectEntered);
        //rayInteractor.
        rayInteractor.selectExited.AddListener(SelectExited);
    }

    private void Update()
    {
        HandleJoystick();
        HandleRayInteractor();
    }

    public void HandleRayInteractor()
    {
        UnityEngine.EventSystems.RaycastResult result;
        if(rayInteractor.TryGetCurrentUIRaycastResult(out result))
        {
            currentUIElement = result.gameObject;
            VRDebug.SetLog(result.gameObject.name);
        }
        else
        {
            VRDebug.SetLog("NONE");
            currentUIElement = null;
        }
    }

    public void HoverEntered(HoverEnterEventArgs eventArgs)
    {
        VRDebug.Log($"HoverEntered {eventArgs.interactor.name}");
    }
    public void HoverExited(HoverExitEventArgs eventArgs)
    {
        VRDebug.Log($"HoverExited {eventArgs.interactor.name}");
    }
    public void SelectEntered(SelectEnterEventArgs eventArgs)
    {
        VRDebug.Log($"SelectEntered {eventArgs.interactor.name}");
    }
    public void SelectTargeted(XRBaseInteractable eventArgs)
    {
        //VRDebug.Log($"HoverEntered {eventArgs.interactor.name}");
    }
    public void SelectExited(SelectExitEventArgs eventArgs)
    {
        VRDebug.Log($"SelectExited {eventArgs.interactor.name}");
    }

    private void HandleJoystick()
    {
        joystickDir = joystick.action.ReadValue<Vector2>();
    }

    public ButtonHandler FindButtonHandler(InputActionReference button)
    {
        foreach (ButtonHandler handler in allButtonHandlers)
        {
            if (handler.button == button)
                return handler;
        }

        ButtonHandler newHandler = new ButtonHandler(button);
        allButtonHandlers.Add(newHandler);

        return newHandler;
    }

    [System.Serializable]
    public class ButtonHandler
    {
        public InputActionReference button;
        public event Action<InputAction.CallbackContext> OnButtonDown;
        public event Action<InputAction.CallbackContext> OnButtonUp;
        public bool isPressed;


        public ButtonHandler(InputActionReference button)
        {
            this.button = button;
            isPressed = false;

            button.action.performed += ActionPerformed;
            button.action.canceled += ActionCanceled;
        }

        public void ActionPerformed(InputAction.CallbackContext context)
        {
            isPressed = true;
            OnButtonDown?.Invoke(context);
        }
        public void ActionCanceled(InputAction.CallbackContext context)
        {
            isPressed = false;
            OnButtonUp?.Invoke(context);
        }
    }
}
