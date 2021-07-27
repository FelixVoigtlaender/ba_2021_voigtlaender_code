using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHover : MonoBehaviour
{
    public Vector3 offset = Vector3.up;
    private void Update()
    {
        if (!transform.parent)
            return;

        // Hover above
        transform.position = transform.parent.position + offset;

        // Look at camera
        Vector3 forward = (transform.position - Camera.main.transform.position);
        forward.y = 0;
        transform.forward = forward;
    }

    private void OnValidate()
    {
        Update();
    }
}
