using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using UnityEngine.XR;
using UnityEngine.InputSystem;
public class fvInputModeManager : MonoBehaviour
{
    public static fvInputModeManager instance;
    List<DisplayButton> displayButtons;
    public List<Mode> modes = new List<Mode>();


    public Stack<Mode> modeStack = new Stack<Mode>();

    public event Action<Mode> OnModeChanged;


    public void Awake()
    {
        instance = this;
        DisplayButton[] displayButtons = GetComponentsInChildren<DisplayButton>();
        this.displayButtons = new List<DisplayButton>(displayButtons);
    }

    public void PreviousMode()
    {
        if (modeStack.Count <= 1)
            return;

        modeStack.Pop();
        SwitchMode(modeStack.Peek().name, false);
    }
    public void SwitchMode(string modeName, bool addToStack = true)
    {
        foreach(DisplayButton displayButton in displayButtons)
        {
            displayButton.SetButtonText("");
        }
        foreach (Mode mode in modes)
        {
            mode.isActive = false;
        }
        Mode newMode = FindMode(modeName);
        StartCoroutine(ActivateMode(newMode));
    }
    IEnumerator ActivateMode(Mode newMode)
    {
        yield return new WaitForEndOfFrame();
        if (newMode != null)
            newMode.isActive = true;

        OnModeChanged?.Invoke(newMode);

    }

    public ButtonModeHandler AddButtonMode(InputActionReference button, string buttonText, string modeName)
    {
        Mode mode = FindMode(modeName);
        DisplayButton displayButton = FindDisplayButton(button);

        ButtonModeHandler handler = new ButtonModeHandler(button, mode, displayButton, buttonText);
        mode.buttonModeHandlers.Add(handler);
        return handler;
    }
    
    public ButtonModeHandler AddButtonMode(ButtonSettings buttonSettings)
    {
        ButtonModeHandler handler =
            AddButtonMode(buttonSettings.button, buttonSettings.buttonText, buttonSettings.modeName);
        buttonSettings.handler = handler;
        return handler;
    }

    public Mode FindMode(string modeName)
    {
        foreach (Mode mode in modes)
        {
            if (mode.name == modeName)
                return mode;
        }
        Mode newMode = new Mode(modeName);
        modes.Add(newMode);

        return newMode;
    }
    public DisplayButton FindDisplayButton(InputActionReference button)
    {
        foreach(DisplayButton displayButton in displayButtons)
        {
            if (displayButton.button == button)
                return displayButton;
        }

        return null;
    }


    [Serializable]
    public class ButtonSettings
    {
        public InputActionReference button;
        public string modeName = "";
        public string buttonText = "";

        private ButtonModeHandler _handler;

        public ButtonModeHandler handler
        {
            get
            {
                if (_handler == null)
                {
                    _handler = fvInputModeManager.instance.AddButtonMode(this);
                }
                return _handler;
            }
            set
            {
                _handler = value;
            }
        }
    }

    public class Mode
    {
        public string name = "";
        public event Action<bool> onModeChange;
        public List<ButtonModeHandler> buttonModeHandlers = new List<ButtonModeHandler>();
        [SerializeField]
        bool _isActive = false;

        List<ButtonModeHandler> buttons = new List<ButtonModeHandler>();
        
        public bool isActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (value == _isActive)
                    return;
                _isActive = value;
                onModeChange?.Invoke(value);

            }
        }
        public Mode(string name)
        {
            this.name = name;
        }

    }
    [System.Serializable]
    public class ButtonModeHandler
    {
        public string buttonText = "";
        public DisplayButton displayButton;
        public InputActionReference button;
        public event Action<InputAction.CallbackContext> OnButtonDown;
        public event Action<InputAction.CallbackContext> OnButtonUp;
        public bool isPressed;
        public Mode mode;

        public ButtonModeHandler(InputActionReference button, Mode mode, DisplayButton displayButton, string buttonText)
        {
            this.button = button;
            isPressed = false;
            this.mode = mode;
            mode.onModeChange += OnModeChanged;

            this.displayButton = displayButton;
            this.buttonText = buttonText;

            button.action.performed += ActionPerformed;
            button.action.canceled += ActionCanceled;
        }

        public void ActionPerformed(InputAction.CallbackContext context)
        {
            if (!mode.isActive)
                return;

            isPressed = true;
            OnButtonDown?.Invoke(context);
        }
        public void ActionCanceled(InputAction.CallbackContext context)
        {
            if (!mode.isActive)
                return;

            OnButtonUp?.Invoke(context);
            isPressed = false;
        }

        public void OnModeChanged(bool value)
        {
            if (value)
            {
                displayButton.SetButtonText(buttonText);
            }
            else
            {
                if (isPressed)
                {

                    isPressed = false;
                    OnButtonUp?.Invoke(new InputAction.CallbackContext());
                }
            }
        }
    }

}
