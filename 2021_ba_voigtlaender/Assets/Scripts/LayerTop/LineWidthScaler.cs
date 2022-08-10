using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LineWidthScaler : MonoBehaviour
{
    public float width = 0.05f;
    private XRRig _xrRig;
    XRRig xrRig
    {
        get
        {
            if (_xrRig)
                return _xrRig;
            _xrRig = FindObjectOfType<XRRig>();
            return _xrRig;
        }
    }

    private LineRenderer _line;

    LineRenderer line
    {
        get
        {
            if (_line)
                return _line;
            _line = GetComponent<LineRenderer>();
            return _line;
        }
    }
    


    private void FixedUpdate()
    {

        float scale = xrRig.transform.localScale.x * width;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, scale);
        curve.AddKey(1, scale);
 
        line.widthCurve = curve;
    }
}
