using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;


[Serializable]
public class BoolEvent :  UnityEvent<bool>{ }
public class BetterToggle : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Objects")]
    public GameObject on;
    public GameObject off;
    [SerializeField]
    private bool _isOn;
    [Header("Events")]
    public BoolEvent OnValueChanged;
    public UnityEvent OnValueTrue;
    public UnityEvent OnValueFalse;

    bool abort = false;

    public bool isOn
    {
        get { return _isOn; }
        set 
        {
            _isOn = value;
            if (on)
                on.SetActive(value);
            if (off)
                off.SetActive(!value);

            OnValueChanged?.Invoke(value);
            if (value)
                OnValueTrue?.Invoke();
            else
                OnValueFalse?.Invoke();

        }
    }
    private void Start()
    {
        isOn = isOn;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        abort = false;
    }


    public void OnDrag(PointerEventData eventData)
    {
        abort = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(!abort)
            Toggle();

        abort = false;
    }

    public void Toggle()
    {
        isOn = !isOn;

    }

    public void SetWithoutNotify(bool value)
    {
        _isOn = value;
        if (on)
            on.SetActive(value);
        if (off)
            off.SetActive(!value);
    }



#if UNITY_EDITOR
    public void OnValidate()
    {
        if (on)
            on.SetActive(_isOn);
        if (off)
            off.SetActive(!_isOn);
    }

#endif
}
