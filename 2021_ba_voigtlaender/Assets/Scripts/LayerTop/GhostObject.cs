using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
public class GhostObject : MonoBehaviour
{
    [Header("Visuals")]
    public Material material;
    public LineRenderer lineRenderer;
    MeshFilter myMeshFilter;
    MeshRenderer myMeshRenderer;
    [Header("Data")]
    public DatTransform datTransform;
    public DatRecording datRecording;
    Coroutine playCoroutine;
    Coroutine recordCoroutine;
    Action onAbort;
    Action onComplete;
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
        datRecording = null;
        datTransform = null;
        lineRenderer.positionCount = 0;
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
            return;
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

    public void Setup(DatRecording datRecording)
    {
        Reset();

        DatTransform datTransform = datRecording.datTransform;
        this.datRecording = datRecording;
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
        Collider cloneCollider = (Collider)collider.CopyComponent(gameObject);
        cloneCollider.isTrigger = true;
        // Set Transform

        transform.position = datTransform.datPosition.Value;
        transform.rotation = datTransform.datRotation.Value;
        transform.localScale = datTransform.datLocalScale.Value + skin;


        lineRenderer.positionCount = datRecording.Value.Count;
        for (int i = 0; i < datRecording.Value.Count; i++)
        {
            lineRenderer.SetPosition(i, datRecording.Value[i].datPosition.Value);
        }
    }

    public void Record(bool isRecording, Action OnAbort)
    {
        if (isRecording)
        {
            Play(false, null);
            recordCoroutine = StartCoroutine(Record(24, datRecording));
            onAbort = OnAbort;
        }
        else if(recordCoroutine != null)
        {
            StopCoroutine(recordCoroutine);
            onAbort?.Invoke();
        }
    }
    public void Play(bool isPlaying, Action OnCompleted)
    {
        if (isPlaying)
        {
            Record(false,null);

            onComplete = OnCompleted;
            playCoroutine = StartCoroutine(Play(24, datRecording));
        }
        else if(playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            onComplete?.Invoke();
        }

    }

    private IEnumerator Record(float fps, DatRecording datRecording)
    {

        int maxSeconds = 360;

        datRecording.Value = new List<DatTransform>();
        lineRenderer.positionCount = 0;
        while (true)
        {
            yield return new WaitForSeconds(1f / fps);

            DatVector3 datPosition = new DatVector3(transform.position);
            DatQuaternion datRotation = new DatQuaternion(transform.rotation);
            DatVector3 datLocalScale = new DatVector3(transform.localScale);
            DatObj datObj = datRecording.datTransform.datObj;


            DatTransform datTransform = new DatTransform(datObj, datPosition, datRotation, datLocalScale);
            datRecording.AddDatTransform(datTransform);

            //VRDebug.Log("RECORDING " +isRecording);
            lineRenderer.positionCount = datRecording.Value.Count;
            lineRenderer.SetPosition(datRecording.Value.Count - 1, datPosition.Value);


            if (datRecording.Value.Count * (1/fps) > maxSeconds)
            {
                onAbort?.Invoke();
                break;
            }
        }
    }
    private IEnumerator Play(float fps, DatRecording datRecording)
    {
        List<DatTransform> recording = datRecording.Value;
        for (int i = 0; i < recording.Count; i++)
        {
            DatVector3 datPosition = recording[i].datPosition;
            DatQuaternion datRotation = recording[i].datRotation;
            DatVector3 datLocalScale = recording[i].datLocalScale;

            float stepTime = 1f / fps;
            transform.DOMove(datPosition.Value, stepTime);
            transform.DORotateQuaternion(datRotation.Value, stepTime);
            transform.DOScale(datLocalScale.Value, stepTime);
            yield return new WaitForSeconds(stepTime);
        }
        onComplete?.Invoke();
    }
}
