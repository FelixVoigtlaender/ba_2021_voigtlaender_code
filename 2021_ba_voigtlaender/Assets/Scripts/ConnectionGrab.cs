using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConnectionGrab : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference button;
    fvInputManager inputManager;
    fvInputManager.ButtonHandler handler;

    [Header("Connection")]
    public GameObject connectionPrefab;
    BezierCurve currentConnection;
    float distance;
    

    private void Awake()
    {
        inputManager = GetComponent<fvInputManager>();

        handler = inputManager.FindButtonHandler(button);
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
        GameObject startObj = inputManager.currentUIElement;
        if (!startObj)
            return;

        currentConnection = InitConnection(startObj.transform);
        //Set start
        ConnectBezier(currentConnection.start, startObj.transform);

        //Set end
        currentConnection.end.transform.position = startObj.transform.position;
        distance = (currentConnection.end.transform.position - transform.position).magnitude;
    }

    public void HandleDrag()
    {
        if (!handler.isPressed)
            return;
        if (currentConnection == null)
            return;

        GameObject endObj = inputManager.currentUIElement;
        if (endObj)
        {
            currentConnection.end.transform.position = endObj.transform.position;
            currentConnection.end.transform.rotation = endObj.transform.rotation;
        }
        else
        {
            currentConnection.end.transform.position = transform.position + transform.forward * distance;
        }
    }

    public void OnButtonUp(InputAction.CallbackContext context)
    {
        GameObject endObj = inputManager.currentUIElement;
        if (!endObj)
        {
            if (currentConnection)
                Destroy(currentConnection.gameObject);

            currentConnection = null;
            return;
        }


        ConnectBezier(currentConnection.end, endObj.transform);


        currentConnection = null;
    }



    public BezierCurve InitConnection(Transform start)
    {
        GameObject connectionObj = Instantiate(connectionPrefab, start.position, Quaternion.identity);
        BezierCurve bezier = connectionObj.GetComponent<BezierCurve>();
        return bezier;
    }

    public void ConnectBezier(BezierCurve.BezierConnection connection, Transform parent)
    {
        connection.transform.parent = parent;
        connection.transform.localPosition = Vector3.zero;
        connection.transform.localRotation = Quaternion.identity;
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

        currentConnection.start.dir = GetLocalNormals(currentConnection.start.transform.position, currentConnection.end.transform.position, up, right);
        currentConnection.end.dir = GetLocalNormals(currentConnection.end.transform.position, currentConnection.start.transform.position, up, right);

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
