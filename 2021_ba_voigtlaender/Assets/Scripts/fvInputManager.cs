using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

[RequireComponent(typeof(XRController))]
public class fvInputManager : MonoBehaviour
{
    public List<ButtonHandler> allButtonHandlers = new List<ButtonHandler>();
    private XRController controller;
    private void Awake()
    {
        controller = GetComponent<XRController>();
    }

    private void Update()
    {
        HandleButtonEvents();
    }

    private void HandleButtonEvents()
    {
        foreach(ButtonHandler handler in allButtonHandlers)
        {
            handler.HandleState(controller);
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
