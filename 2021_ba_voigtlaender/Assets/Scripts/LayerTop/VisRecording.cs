using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisRecording : MonoBehaviour
{
    DatRecording datRecording;
    public void Setup(DatRecording datRecording)
    {
        this.datRecording = datRecording;
    }
}
