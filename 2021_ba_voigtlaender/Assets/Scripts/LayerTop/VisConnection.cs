using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisConnection : MonoBehaviour
{
    public BezierCurve bezierCurve;
    public VRConnection vrConnection;

    VisPort startVisPort;
    VisPort endVisPort;
    Vector3 lastPosition;
    public void Setup(VisPort start)
    {
        vrConnection = VRManager.instance.InitVRConnection(start.vrPort, null, false);
        bezierCurve.start.Connect(start.transform);
        bezierCurve.end.Hover(start.transform);
        startVisPort = start;
        bezierCurve.SetColor(start.vrPort.dataType.GetColor());

        lastPosition = start.transform.position;
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
            VRVariable vrVariable = VRManager.instance.InitVRVariable(startVisPort.vrPort.dataType, false);
            GameObject objVisVariable = VisManager.instance.InitVRLogicElement(vrVariable);
            objVisVariable.transform.position = lastPosition;
            if (!objVisVariable)
                return;
            VisVariable visVariable = objVisVariable.GetComponent<VisVariable>();
            visVariable.Setup(vrVariable);

            if (startVisPort.vrPort.type == PortType.INPUT)
                endVisPort = visVariable.visOutPorts[0];
            else
                endVisPort = visVariable.visInPorts[0];
        }


        VisPort visPortA = startVisPort;
        VisPort visPortB = endVisPort;

        vrConnection.Connect(visPortA.vrPort, visPortB.vrPort);
        startVisPort = visPortA.vrPort == vrConnection.start ? visPortA : visPortB;
        endVisPort = visPortA.vrPort == vrConnection.end ? visPortA : visPortB;

        bezierCurve.start.Connect(startVisPort.transform);
        bezierCurve.end.Connect(endVisPort.transform);
    }
}
