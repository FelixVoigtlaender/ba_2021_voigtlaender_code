using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostObject : MonoBehaviour
{
    public Material material;
    public LineRenderer lineRenderer;
    MeshFilter myMeshFilter;
    MeshRenderer myMeshRenderer;
    // Data
    public DatTransform datTransform;

    Vector3 skin = Vector3.one * 0.01f;

    private void Awake()
    {
        myMeshFilter = GetComponent<MeshFilter>();
        myMeshRenderer = GetComponent<MeshRenderer>();
        myMeshRenderer.material = material;
    }

    private void Reset()
    {
        if(TryGetComponent(out Collider collider))
        {
            Destroy(collider);
        }
    }

    private void Update()
    {
        if(datTransform != null)
        {
            if (datTransform.datPosition.Value != transform.position)
                datTransform.datPosition.Value = transform.position;
            if (datTransform.datRotation.Value != transform.rotation)
                datTransform.datRotation.Value = transform.rotation;
            if (datTransform.datLocalScale.Value != transform.localScale- skin)
                datTransform.datLocalScale.Value = transform.localScale - skin;


            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, datTransform.datObj.Value.gameObject.transform.position);
        }
    }

    public void Setup(DatTransform datTransform)
    {
        Reset();



        // Get other Components
        GameObject otherObject = datTransform.datObj.Value.gameObject;
        MeshFilter otherMeshFilter = otherObject.GetComponent<MeshFilter>();
        MeshRenderer otherMeshRenderer = otherObject.GetComponent<MeshRenderer>();
        Collider collider = otherObject.GetComponent<Collider>();

        // Interrupt if there are no mesh and meshrenderer
        if (!otherMeshFilter || !otherMeshRenderer)
        {
            gameObject.SetActive(false);
            VRDebug.Log("No mesh(filter/renderer) found!");
            return;
        }

        // Set Mesh and Collider
        myMeshFilter.sharedMesh = otherMeshFilter.sharedMesh;
        Collider cloneCollider  = (Collider)collider.CopyComponent(gameObject);
        cloneCollider.isTrigger = true;
        // Set Transform
        
        transform.position = datTransform.datPosition.Value;
        transform.rotation = datTransform.datRotation.Value;
        transform.localScale = datTransform.datLocalScale.Value + skin;

        this.datTransform = datTransform;
    }
}
