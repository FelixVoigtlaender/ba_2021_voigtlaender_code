using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisConnection : MonoBehaviour
{
    [Header("Setup")]
    public BezierCurve bezierCurve;
    public VRConnection vrConnection;
    [Header("Colors")]
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;
    public Color errorColor = Color.red;
    public float activeWidth = 0.02f;
    public float inactiveWidth = 0.01f;
    Color normalColor;

    VisPort startVisPort;
    VisPort endVisPort;
    Vector3 lastPosition;


    public void Setup(VisPort start)
    {
        vrConnection = VRManager.instance.InitVRConnection(start.vrPort, null, false);
        vrConnection.OnDelete += OnDelete;
        bezierCurve.start.Connect(start.transform);
        bezierCurve.end.Hover(start.transform);
        startVisPort = start;
        normalColor = start.vrPort.dataType.GetColor();
        bezierCurve.SetColor(normalColor);
        bezierCurve.SetWidth(inactiveWidth);

        lastPosition = start.transform.position;
        vrConnection.OnActive += OnActive;


        ResetColor();

        StartCoroutine(ResetActive(0.3f));
    }

    public void Setup(VRConnection vrConnection)
    {
        this.vrConnection = vrConnection; 
        
        normalColor = vrConnection.start.dataType.GetColor();
        bezierCurve.SetColor(normalColor);
        bezierCurve.SetWidth(inactiveWidth);
        vrConnection.OnActive += OnActive;
        vrConnection.OnDelete += OnDelete;
        ResetColor();
        StartCoroutine(ResetActive(0.3f));

        StartCoroutine(ConnectVisPorts(vrConnection));
    }
    IEnumerator ConnectVisPorts(VRConnection vrConnection)
    {
        while (!startVisPort && !endVisPort)
        {
            yield return new WaitForSeconds(1);
            VisPort[] visPorts = FindObjectsOfType<VisPort>();
            foreach(VisPort visPort in visPorts)
            {
                if(visPort.vrPort == vrConnection.start)
                {
                    startVisPort = visPort;

                }
                if (visPort.vrPort == vrConnection.end)
                {
                    endVisPort = visPort;
                }
            }
        }
        Release();
    }

    public void OnActive(VRData vrData)
    {

        bezierCurve.SetWidth(activeWidth);
        if (vrData == null)
        {
            bezierCurve.SetColor(normalColor * errorColor);
        }
        else
        {
            bezierCurve.SetColor(normalColor * activeColor);
        }
    }

    private IEnumerator ResetActive(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            ResetColor();
            bezierCurve.SetWidth(inactiveWidth);
        }
    }
    public void ResetColor()
    {
        bezierCurve.SetColor(normalColor * inactiveColor);
    }

    public void Drag(Vector3 position, GameObject uiObject)
    {
        if(uiObject && uiObject.TryGetComponent(out VisPort visPort))
        {
            if (vrConnection.CanConnect(visPort.vrPort))
            {
                bezierCurve.end.Hover(uiObject.transform);
                endVisPort = visPort;
                return;
            }
        }

        lastPosition = position;
        endVisPort = null;
        bezierCurve.end.transform.position = position;
    }

    public void Release()
    {
        if (!endVisPort)
        {
            Delete();
            return;

            VRVariable vrVariable = VRManager.instance.InitVRVariable(startVisPort.vrPort.dataType, false);
            GameObject objVisVariable = VisManager.instance.InitVRLogicElement(vrVariable);

            Canvas canvas = objVisVariable.GetComponentInParent<Canvas>();
            canvas.transform.position = lastPosition;



            if (!objVisVariable)
                return;
            VisVariable visVariable = objVisVariable.GetComponent<VisVariable>();
            visVariable.Setup(vrVariable);

            if (startVisPort.vrPort.portType == PortType.INPUT)
                endVisPort = visVariable.visOutPorts[0];
            else
                endVisPort = visVariable.visInPorts[0];
        }


        VisPort visPortA = startVisPort;
        VisPort visPortB = endVisPort;

        bool isConnected = vrConnection.Connect(visPortA.vrPort, visPortB.vrPort);
        startVisPort = visPortA.vrPort == vrConnection.start ? visPortA : visPortB;
        endVisPort = visPortA.vrPort == vrConnection.end ? visPortA : visPortB;

        bezierCurve.start.Connect(startVisPort.transform);
        bezierCurve.start.dynamicNormals = false;
        bezierCurve.start.normal = Vector3.right;

        bezierCurve.end.Connect(endVisPort.transform);
        bezierCurve.end.dynamicNormals = false;
        bezierCurve.end.normal = Vector3.left;

        
        
        vrConnection.Save();
        
        VRDebug.Log($"Connected {isConnected}");
    }

    public void Delete()
    {
        vrConnection?.Delete();
    }

    private void OnDelete()
    {
        VRDebug.Log("Deleting VisConnection");

        bezierCurve?.Delete();
        if (gameObject!=null)
            Destroy(gameObject);


        if (vrConnection != null)
            vrConnection.OnDelete -= OnDelete;
    }

    public void OnDestroy()
    {
        if (vrConnection == null)
            return;

        bezierCurve?.Delete();
        vrConnection.OnDelete -= OnDelete;
        vrConnection.OnActive -= OnActive;
    }
}
