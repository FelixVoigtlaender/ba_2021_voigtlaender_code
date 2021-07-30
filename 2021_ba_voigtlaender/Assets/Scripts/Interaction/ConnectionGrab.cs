using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ConnectionGrab : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference button;
    public string modeName = "";
    public string buttonText = "";
    fvInputManager inputManager;

    fvInputModeManager inputModeManager;
    fvInputModeManager.ButtonModeHandler handler;


    [Header("Object")]
    public LayerMask objectLayer;

    [Header("Connection")]
    public GameObject connectionPrefab;
    BezierCurve currentConnection;
    float distance;

    VisConnection currentVisConnection;
    

    private void Start()
    {
        inputManager = GetComponentInParent<fvInputManager>();
        inputModeManager = GetComponentInParent<fvInputModeManager>();

        handler = inputModeManager.AddButtonMode(button, buttonText, modeName);
        handler.OnButtonDown += OnButtonDown;
        handler.OnButtonUp += OnButtonUp;
    }
    public void Update()
    {
        HandleDrag();
        //HandleNormals();
    }

    public void OnButtonDown(InputAction.CallbackContext context)
    {
        if (!inputManager.isUIHitClosest || !inputManager.uiRaycastHit.HasValue)
            return;

        GameObject startObj = inputManager.uiRaycastHit.Value.gameObject;
        if (!startObj.TryGetComponent(out VisPort visPort))
            return;

        GameObject objVisConnection =Instantiate(VisManager.instance.prefabVisConnection);
        currentVisConnection = objVisConnection.GetComponent<VisConnection>();
        currentVisConnection.Setup(visPort);

        distance = (visPort.transform.position - transform.position).magnitude;
    }

    public void HandleDrag()
    {
        if (!handler.isPressed)
            return;
        if (currentVisConnection == null)
            return;

        GameObject endObj = !inputManager.uiRaycastHit.HasValue ? null : inputManager.uiRaycastHit.Value.gameObject;
        Vector3 position = transform.position + transform.forward * distance;
        currentVisConnection.Drag(position, endObj);
    }

    public void OnButtonUp(InputAction.CallbackContext context)
    {
        if (!currentVisConnection)
            return;
        currentVisConnection.Release();
        currentVisConnection = null;
    }



    public BezierCurve InitConnection(Transform start)
    {
        GameObject connectionObj = Instantiate(connectionPrefab, start.position, Quaternion.identity);
        BezierCurve bezier = connectionObj.GetComponent<BezierCurve>();
        return bezier;
    }


    public void HandleNormals()
    {
        if (!currentConnection)
            return;

        Vector3 deltaStart = currentConnection.start.transform.position - Camera.main.transform.position;
        Vector3 deltaEnd = currentConnection.end.transform.position - Camera.main.transform.position;

        Vector3 deltaMid = (deltaStart.normalized + deltaEnd.normalized).normalized;
        Vector3 up = Vector3.up;
        Vector3 right = Vector3.Cross(up, deltaMid).normalized;

        currentConnection.start.normal = GetLocalNormals(currentConnection.start.transform.position, currentConnection.end.transform.position, up, right);
        currentConnection.end.normal = GetLocalNormals(currentConnection.end.transform.position, currentConnection.start.transform.position, up, right);

    }

    public Vector3 GetLocalNormals(Vector3 start, Vector3 end, Vector3 up, Vector3 right)
    {
        float horizontal = Vector3.Dot(right, end - start);
        float vertical = Vector3.Dot(up, end - start);


        Vector3 normal = Vector3.zero;
        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            //Horizontal
            normal = Vector3.right * Mathf.Sign(horizontal);
        }
        else
        {
            //Vertical
            normal = Vector3.up * Mathf.Sign(vertical);
        }
        return normal;
    }

}
