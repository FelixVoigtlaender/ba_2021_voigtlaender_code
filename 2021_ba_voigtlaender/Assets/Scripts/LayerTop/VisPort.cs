using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisPort : MonoBehaviour
{
    public VRPort vrPort;
    public RawImage image;
    public void Setup(VRPort vrPort)
    {
        this.vrPort = vrPort;
        image.color = vrPort.dataType.GetColor();
    }
}
