using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using UnityEngine.XR;

[RequireComponent(typeof(XRController))]
public class fvInputManager : MonoBehaviour
{
    public List<ButtonHandler> allButtonHandlers = new List<ButtonHandler>();
    private XRController controller;
    private XRRayInteractor rayInteractor;

    public Vector2 joystickDir;
    public Vector3 relativeJoystickDir;
    private void Awake()
    {
        controller = GetComponent<XRController>();
        rayInteractor = GetComponent<XRRayInteractor>();

        rayInteractor.hoverEntered.AddListener(HoverEntered);
        rayInteractor.hoverExited.AddListener(HoverExited);
        rayInteractor.selectEntered.AddListener(SelectEntered);
        //rayInteractor.
        rayInteractor.selectExited.AddListener(SelectExited);
    }

    private void Update()
    {
        HandleButtonEvents();
        HandleJoystick();
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

    private void HandleButtonEvents()
    {
        foreach(ButtonHandler handler in allButtonHandlers)
        {
            handler.HandleState(controller);
        }
    }
    private void HandleJoystick()
    {
        Vector2 primary2dValue;
        InputFeatureUsage<Vector2> primary2DVector = CommonUsages.primary2DAxis;
        if(controller.inputDevice.TryGetFeatureValue(primary2DVector, out primary2dValue))
        {
            relativeJoystickDir = (transform.forward * primary2dValue.y + transform.right * primary2dValue.x);
            joystickDir = primary2dValue;

            Debug.DrawRay(transform.position, relativeJoystickDir * 0.1f);
        }
        else
        {
            joystickDir = relativeJoystickDir = Vector2.zero;
        }
    }

    public ButtonHandler FindButtonHandler(InputHelpers.Button button)
    {
        foreach (ButtonHandler handler in allButtonHandlers)
        {
            if (handler.button == button)
                return handler;
        }

        ButtonHandler newHandler = new ButtonHandler();
        newHandler.button = button;
        allButtonHandlers.Add(newHandler);

        return newHandler;
    }

    [System.Serializable]
    public class ButtonHandler
    {
        public InputHelpers.Button button = InputHelpers.Button.None;
        public event Action<XRController> OnButtonDown;
        public event Action<XRController> OnButtonUp;
        public bool pressed;
        private bool previousPress = false;

        public void HandleState(XRController controller)
        {
            if(controller.inputDevice.IsPressed(button, out pressed, controller.axisToPressThreshold))
            {
                if (previousPress != pressed)
                {
                    previousPress = pressed;

                    if (pressed)
                    {
                        OnButtonDown?.Invoke(controller);
                    }
                    else
                    {
                        OnButtonUp?.Invoke(controller);
                    }
                }
            }
        }
    }
}
