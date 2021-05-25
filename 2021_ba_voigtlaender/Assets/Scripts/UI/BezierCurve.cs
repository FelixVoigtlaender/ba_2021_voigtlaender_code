using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{

	public BezierConnection start;
	public BezierConnection end;
	public int pointCount = 100;
	LineRenderer line;

	public void Awake()
    {
		line = GetComponent<LineRenderer>();
    }



    public void Update()
    {
		UpdateBezier();
    }
	public void UpdateBezier()
    {
		if (start == null || !start.transform)
			return;
		if (end == null || !end.transform)
			return;
		Vector3 p0 = start.transform.position + start.offset;
		Vector3 p1 = p0 + start.dir;
		Vector3 p3 = end.transform.position + end.offset;
		Vector3 p2 = p3 + end.dir;

		line.positionCount = pointCount;
		for(int i = 0; i < pointCount; i++)
        {
			Vector3 point = BezierPathCalculation(p0, p1, p2, p3, (float)i / (float)pointCount);
			line.SetPosition(i, point);

		}
    }

    private void OnDrawGizmos()
    {
		if (start == null || !start.transform)
			return;
		if (end == null || !end.transform)
			return;


		Vector3 p0 = start.transform.position + start.offset;
		Vector3 p1 = p0 + start.dir;
		Vector3 p2 = end.transform.position + end.offset;
		Vector3 p3 = p2 + end.dir;

		Gizmos.color = Color.green;
		Gizmos.DrawLine(p0, p1);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(p2, p3);
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

	[System.Serializable]
	public class BezierConnection
    {
		public Transform transform;
		public Vector3 offset;
		public Vector3 dir;
    }
}
