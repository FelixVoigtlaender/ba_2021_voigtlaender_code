using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{
	[Header("Line")]
	public LineRenderer line;
	public int pointCount = 100;
	[Header("Start/End")]
	public BezierConnection start;
	public BezierConnection end;

	public float width = 1;

	public void Awake()
    {
		line = GetComponent<LineRenderer>();
    }


	public void SetColor(Color color)
    {
		line.endColor = line.startColor = color;
    }
	public void SetWidth(float width)
	{
		this.width = width;
	}

	public void Update()
    {
	    if(line)
		    line.startWidth = line.endWidth = width * transform.lossyScale.x;
	    
		UpdateBezier();
		HandleNormals();

	}
	public void UpdateBezier()
    {
		if (!line)
			return;
		if (start == null || !start.transform)
			return;
		if (end == null || !end.transform)
			return;

		float distance = (start.transform.position - end.transform.position).magnitude;
		float dirMagnitude = distance / 2;



		Vector3 p0 = start.transform.position + start.offset;
		Vector3 p3 = end.transform.position + end.offset;

		Vector3 p0Direction = start.normal.normalized * dirMagnitude;
		p0Direction = start.useLocalSpace ? start.transform.TransformDirection(start.normal).normalized * dirMagnitude : p0Direction;
		Vector3 p3Direction = end.normal.normalized * dirMagnitude;
		p3Direction = end.useLocalSpace ? end.transform.TransformDirection(end.normal).normalized * dirMagnitude : p3Direction;

		Vector3 p1 = p0 + p0Direction;
		Vector3 p2 = p3 + p3Direction;

		Debug.DrawLine(p0, p1, Color.green);
		Debug.DrawLine(p2, p3, Color.red);

		line.positionCount = pointCount;
		for(int i = 0; i < pointCount; i++)
        {
			Vector3 point = BezierPathCalculation(p0, p1, p2, p3, (float)i / (float)(pointCount-1));
			line.SetPosition(i, point);

		}
    }

    private void OnValidate()
    {
		Update();
    }



    public void HandleNormals()
	{
		if (start == null || !start.transform)
			return;
		if (end == null || !end.transform)
			return;
		if (!Camera.main)
			return;

		Vector3 deltaStart = start.transform.position - Camera.main.transform.position;
		Vector3 deltaEnd = end.transform.position - Camera.main.transform.position;

		Vector3 deltaMid = (deltaStart.normalized + deltaEnd.normalized).normalized;
		Vector3 up = Vector3.up;
		Vector3 right = Vector3.Cross(up, deltaMid).normalized;

		if(start.dynamicNormals)
			start.normal = GetLocalNormals(start.transform.position, end.transform.position, up, right);
		if(end.dynamicNormals)
			end.normal = GetLocalNormals(end.transform.position, start.transform.position, up, right);

	}


    Vector3 BezierPathCalculation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		float tt = t * t;
		float ttt = t * tt;
		float u = 1.0f - t;
		float uu = u * u;
		float uuu = u * uu;

		Vector3 B = new Vector3();
		B = uuu * p0;
		B += 3.0f * uu * t * p1;
		B += 3.0f * u * tt * p2;
		B += ttt * p3;

		return B;
	}

	public Vector3 GetLocalNormals(Vector3 start, Vector3 end, Vector3 up, Vector3 right)
	{
		float horizontal = Vector3.Dot(right, end - start);
		float vertical = Vector3.Dot(up, end - start);


		Vector3 normal = Vector3.zero;
		if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
		{
			//Horizontal
			normal = Vector3.right * Mathf.Sign(horizontal);
		}
		else
		{
			//Vertical
			normal = Vector3.up * Mathf.Sign(vertical);
		}
		return normal;
	}

	public void Delete()
    {
		start?.Delete();
		end?.Delete();
    }

	[System.Serializable]
	public class BezierConnection
    {
		public Transform transform;
		public Vector3 offset;
		public Vector3 normal;
		public bool dynamicNormals = true;
		public bool useLocalSpace = true;

		public void Connect(Transform parent)
		{
			transform.parent = parent;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
		public void Hover(Transform parent)
        {
			transform.position = parent.position; 
			transform.rotation = parent.rotation;
		}
		public void Delete()
        {
			if (!transform)
				Destroy(transform);
        }
	}
}
