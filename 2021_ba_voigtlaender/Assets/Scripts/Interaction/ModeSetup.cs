using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModeSetup : MonoBehaviour
{
    public List<ModeButton> modeButtons = new List<ModeButton>();
    // Start is called before the first frame update
    void Start()
    {
        fvInputModeManager inputModeManager = GetComponentInParent<fvInputModeManager>();
        ModeButton initialModeButton = null;
        foreach(ModeButton modeButton in modeButtons)
        {
            modeButton.Setup(inputModeManager);
            if (modeButton.isInitial)
            {
                initialModeButton = modeButton;
            }
        }
        if (initialModeButton != null)
        {
            inputModeManager.SwitchMode(initialModeButton.fromMode);
        }
    }



    [System.Serializable]
    public class ModeButton
    {
        public string fromMode = "";
        public string toMode = "";
        public InputActionReference button;
        fvInputModeManager inputModeManager;
        fvInputModeManager.ButtonModeHandler handler;
        public bool isInitial = false;

        public void Setup(fvInputModeManager inputModeManager)
        {
             handler = inputModeManager.AddButtonMode(button, toMode, fromMode);
             handler.OnButtonDown += OnButtonDown;
            handler.OnButtonUp += OnButtonUp;

            this.inputModeManager = inputModeManager;
        }

        public void OnButtonDown(InputAction.CallbackContext context)
        {
            //inputModeManager.SwitchMode(toMode);
        }
        public void OnButtonUp(InputAction.CallbackContext context)
        {
            if(handler.isPressed)
                inputModeManager.SwitchMode(toMode);
        }
    }
}
