using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    public bool clampUpDown = true;
    public CenterType centerType = CenterType.UICenter;

    private void Update()
    {
        Vector3 forward = GetForward();

        transform.forward = forward;
    }

    public Vector3 GetForward()
    {

        Vector3 forward = Vector3.forward;

        switch (centerType)
        {
            case CenterType.UICenter:
                forward = (transform.position - UICenter.Instance.transform.position);
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