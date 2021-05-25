using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    public bool clampUpDown = true;

    private void Update()
    {
        // Look at camera
        Vector3 forward = (transform.position - Camera.main.transform.position);
        if(clampUpDown)
            forward.y = 0;
        transform.forward = forward;
    }

}
