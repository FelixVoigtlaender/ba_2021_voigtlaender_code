using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
public class DisplayButton : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference button;
    public fvInputManager inputManager;
    fvInputManager.ButtonHandler handler;

    [Header("Output")]
    public BetterToggle betterToggle;
    public UnityEvent OnButtonDown;
    public UnityEvent OnButtonUp;
    private void Awake()
    {

        handler = inputManager.FindButtonHandler(button);
        handler.OnButtonDown += (value) => 
        {
            OnButtonDown?.Invoke();
            betterToggle.isOn = true;
        };
        handler.OnButtonUp += (value) =>
        {
            OnButtonUp?.Invoke();
            betterToggle.isOn = false;
        };

        betterToggle.isOn = false;
    }
}
