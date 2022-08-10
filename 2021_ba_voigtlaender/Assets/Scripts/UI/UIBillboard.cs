using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    public bool clampUpDown = true;
    public CenterType centerType = CenterType.UICenter;

    public bool useParentAngle = true;

    private float smoothVel;
    private void Update()
    {
        Vector3 forward = GetForward();
        Vector3 currentForward = GetCurrentForward();

        //transform.forward = forward;

        if (useParentAngle)
        {
            Vector3 parentForward = transform.parent ? transform.parent.forward : Vector3.forward;
            float angle = Vector3.SignedAngle(parentForward, forward, Vector3.up);
            float currentAngle = Vector3.SignedAngle(parentForward, currentForward, Vector3.up);

            float lerpedAngle = Mathf.SmoothDamp(currentAngle, angle, ref smoothVel, 0.5f);
            transform.localEulerAngles = new Vector3(0, lerpedAngle, 0);
        }
        else
        {
            transform.forward = forward;
        }
        

    }

    public Vector3 GetCurrentForward()
    {
        Vector3 currentForward = transform.forward;
        // Look at camera
        if (clampUpDown)
            currentForward.y = 0;

        return currentForward;
    }

    public Vector3 GetForward()
    {

        Vector3 forward = Vector3.forward;

        switch (centerType)
        {
            case CenterType.UICenter:
                forward = (transform.position - UICenter.Instance.transform.position);
                Debug.DrawRay(transform.position, forward);
                break;
            case CenterType.Camera:
                forward = (transform.position - Camera.main.transform.position);
                break;
        }

        // Look at camera
        if (clampUpDown)
            forward.y = 0;


        return forward;
    }



}
[System.Serializable]
public enum CenterType
{
    Camera, UICenter
}