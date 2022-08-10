using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PanelButton : MonoBehaviour
{
    public Transform panelTransform;



    [Header("Input")]
    public InputActionReference button;
    public string modeName = "";
    public string buttonText = "";
    fvInputManager inputManager;

    fvInputModeManager inputModeManager;
    fvInputModeManager.ButtonModeHandler handler;
    private void Start()
    {

        inputManager = GetComponentInParent<fvInputManager>();
        inputModeManager = GetComponentInParent<fvInputModeManager>();

        handler = inputModeManager.AddButtonMode(button, buttonText, modeName);
        handler.OnButtonDown += OnButtonDown;
        handler.mode.onModeChange += OnModeChanged;
        
        OnModeChanged(false);

    }
    public void OnButtonDown(InputAction.CallbackContext context)
    {
        Vector3 position = transform.position + transform.TransformVector(Vector3.forward * 0.2f);
        panelTransform.DOMove(position, 0.3f);
    }

    public void OnModeChanged(bool value)
    {
        panelTransform.gameObject.SetActive(value);
    }
}
