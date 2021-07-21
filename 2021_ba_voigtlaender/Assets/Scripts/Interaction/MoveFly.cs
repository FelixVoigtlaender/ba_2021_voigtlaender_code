using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MoveFly : MonoBehaviour
{

    public Toggle toggle;
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

    public void Update()
    {

        if (!toggle.isOn)
            return;




        Vector3 difference = inputManager.relativeJoystickDir * speed;

        xrRig.transform.position += new Vector3(difference.x, 0, difference.z);
        xrRig.transform.localScale += Vector3.one * difference.y;

        xrRig.transform.localScale = Mathf.Clamp(xrRig.transform.localScale.x, 1, 10) * Vector3.one;
    }
}
