using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MoveFly : MonoBehaviour
{
    
    [Header("Input")]
    fvInputManager inputManager;
    fvInputManager.ButtonHandler handler;

    [Header("Fly")]
    public Transform xrRig;
    public float speed = 0.01f;

    private void Awake()
    {
        inputManager = GetComponent<fvInputManager>();
    }

}
