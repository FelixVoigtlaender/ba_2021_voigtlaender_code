using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniatureMaker : MonoBehaviour
{
    MeshFilter myMeshFilter;
    MeshRenderer myMeshRenderer;
    private void Awake()
    {
        myMeshFilter = GetComponent<MeshFilter>();
        myMeshRenderer = GetComponent<MeshRenderer>();
    }

    public void CopyMesh(GameObject otherObject)
    {
        // Get other mesh and meshrenderer
        MeshFilter otherMeshFilter = otherObject.GetComponent<MeshFilter>();
        MeshRenderer otherMeshRenderer = otherObject.GetComponent<MeshRenderer>();

        // Interrupt if there are no mesh and meshrenderer
        if (!otherMeshFilter || !otherMeshRenderer) 
        {
            gameObject.SetActive(false);
            VRDebug.Log("No mesh(filter/renderer) found!");
            return;
        }

        myMeshFilter.sharedMesh = otherMeshFilter.sharedMesh;
        myMeshRenderer.material = otherMeshRenderer.material;

        Bounds meshBounds = myMeshFilter.sharedMesh.bounds;
        Vector3 meshSize = meshBounds.size;
        Vector3 objSize = otherObject.transform.lossyScale;

        Vector3 normObjSize = objSize / Mathf.Max(MaxFromVector3(objSize),0.001f);
        Vector3 altMeshSize = new Vector3(normObjSize.x * meshSize.x, normObjSize.y * meshSize.y, normObjSize.z * meshSize.z);
        Vector3 miniSize = normObjSize / Mathf.Max(MaxFromVector3(altMeshSize), 1f);

        transform.localScale = miniSize;

        VRDebug.Log(meshBounds.ToString());
        VRDebug.Log(normObjSize.ToString());
    }

    float MaxFromVector3(Vector3 vector)
    {
        return Mathf.Max(vector.x,vector.y,vector.z);
    }
}
