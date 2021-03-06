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

    XRRig xrRig;

    [Header("Raycasting")]
    public XRRayInteractor rayInteractor;
    public float localRayLength = 1;
    public float relativeRayLength = 1;

    public UnityEngine.EventSystems.RaycastResult? uiRaycastHit;
    public RaycastHit? worldRaycastHit;
    public bool isUIHitClosest;


    [Header("Joystick")]
    public InputActionReference joystick;
    public Vector2 joystickDir;
    public Vector3 relativeJoystickDir;

    ActionBasedController controller;


    private void Awake()
    {
        xrRig = FindObjectOfType<XRRig>();
        controller = GetComponent<ActionBasedController>();

        rayInteractor = GetComponent<XRRayInteractor>();

        rayInteractor.hoverEntered.AddListener(HoverEntered);
        rayInteractor.hoverExited.AddListener(HoverExited);
        rayInteractor.selectEntered.AddListener(SelectEntered);
        //rayInteractor.
        rayInteractor.selectExited.AddListener(SelectExited);



    }
    private void Start()
    {
        fvInputModeManager.instance.OnModeChanged += (value) => 
        {
            controller.SendHapticImpulse(1, 0.2f);
        };
    }

    private void Update()
    {
        HandleJoystick();
        HandleRayInteractor();
    }

    public void HandleRayInteractor()
    {
        relativeRayLength = xrRig.transform.localScale.x * localRayLength;
        rayInteractor.maxRaycastDistance = relativeRayLength;

        int raycastHitIndex;
        int uiRaycastHitIndex;
        if(rayInteractor.TryGetCurrentRaycast(out worldRaycastHit,out raycastHitIndex,out uiRaycastHit,out uiRaycastHitIndex,out isUIHitClosest))
        {
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
        relativeJoystickDir = transform.TransformDirection(new Vector3(joystickDir.x, 0, joystickDir.y)).normalized * joystickDir.magnitude;
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
