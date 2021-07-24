using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HandRemover : MonoBehaviour
{
    public BetterToggle toggle;
    [Header("Input")]
    public InputActionReference button;
    fvInputManager inputManager;
    fvInputManager.ButtonHandler handler;
    
    [Header("Sword")]
    public LineRenderer lineRenderer;
    Vector3 previousB;
    VisConnection[] visConnections;
    private void Awake()
    {
        inputManager = GetComponent<fvInputManager>();

        lineRenderer.enabled = false;
        handler = inputManager.FindButtonHandler(button);
        handler.OnButtonDown += OnButtonDown;
        handler.OnButtonUp += OnButtonUp;
    }
    public void OnButtonDown(InputAction.CallbackContext context)
    {
        if (!toggle.isOn)
            return;

        visConnections = FindObjectsOfType<VisConnection>();
        lineRenderer.enabled = true;
        previousB = transform.position + transform.forward * inputManager.rayInteractor.maxRaycastDistance;
    }
    public void OnButtonUp(InputAction.CallbackContext context)
    {

        lineRenderer.enabled = false;
    }


    public void FixedUpdate()
    {
        if (!toggle.isOn)
            return;
        if (!handler.isPressed)
            return;

        // VisObject Deletion
        if (inputManager.uiRaycastHit.HasValue)
        {

            VisObject visObject = inputManager.uiRaycastHit.Value.gameObject.GetComponentInParent<VisObject>();
            if (visObject)
            {
                visObject.Delete();
            }
            else
            {
                VisLogicElement logicElement = inputManager.uiRaycastHit.Value.gameObject.GetComponentInParent<VisLogicElement>();
                if (logicElement)
                {
                    logicElement.Delete();
                }
            }
        }

        // Line Deletion
        Vector3 mid = transform.position;
        Vector3 b = mid + transform.forward * inputManager.relativeRayLength;
        Vector3 c = previousB;
        previousB = b;

        Polygon polygon = new Polygon(mid,b,c);
        polygon.DrawDebug();
        foreach(VisConnection visConnection in visConnections)
        {
            if (CheckCollision(polygon, visConnection))
                visConnection.Delete();
        }

    }

    public bool CheckCollision(Polygon polygon, VisConnection visConnection)
    {
        if (!visConnection || !visConnection.bezierCurve || !visConnection.bezierCurve.line)
            return false;

        // Get Positions
        Vector3[] positions = new Vector3[visConnection.bezierCurve.line.positionCount];
        visConnection.bezierCurve.line.GetPositions(positions);
        int checkCount = 10;
        int stepSize = positions.Length / checkCount;

        Vector3 lastPoint = positions[0];
        for (int i = stepSize; i < positions.Length; i+=stepSize)
        {
            Vector3 nextPoint = positions[i];

            if (polygon.Intersect(lastPoint, nextPoint))
                return true;

            Debug.DrawLine(lastPoint, nextPoint);

            lastPoint = nextPoint;
        }
        return false;
    }


    public class Polygon
    {
        public Vector3 mid;
        public Vector3 b;
        public Vector3 c;
        public Vector3 normal
        {
            get
            {
                return Vector3.Cross(b - mid, c - mid);
            }
        }
        public Polygon(Vector3 mid, Vector3 b, Vector3 c)
        {
            this.mid = mid;
            this.b = b;
            this.c = c;
        }
        public void DrawDebug()
        {
            Debug.DrawLine(mid, b,Color.red,0.1f);
            Debug.DrawLine(mid, c, Color.red, 0.1f);
            Debug.DrawLine(c, b, Color.white, 0.1f);
        }
        public bool Intersect(Vector3 start, Vector3 end)
        {
            Ray ray = new Ray(start, end - start);
            return Intersect(ray);
        }

        public bool Intersect(Ray ray)
        {
            return Intersect(mid, b, c, ray);
        }
        /// <summary>
        /// Checks if the specified ray hits the triagnlge descibed by p1, p2 and p3.
        /// Möller–Trumbore ray-triangle intersection algorithm implementation.
        /// </summary>
        /// <param name="p1">Vertex 1 of the triangle.</param>
        /// <param name="p2">Vertex 2 of the triangle.</param>
        /// <param name="p3">Vertex 3 of the triangle.</param>
        /// <param name="ray">The ray to test hit for.</param>
        /// <returns><c>true</c> when the ray hits the triangle, otherwise <c>false</c></returns>
        public static bool Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray)
        {
            // Vectors from p1 to p2/p3 (edges)
            Vector3 e1, e2;

            Vector3 p, q, t;
            float det, invDet, u, v;


            //Find vectors for two edges sharing vertex/point p1
            e1 = p2 - p1;
            e2 = p3 - p1;

            // calculating determinant 
            p = Vector3.Cross(ray.direction, e2);

            //Calculate determinat
            det = Vector3.Dot(e1, p);

            float Epsilon = Mathf.Epsilon;
            //if determinant is near zero, ray lies in plane of triangle otherwise not
            if (det > -Epsilon && det < Epsilon) { return false; }
            invDet = 1.0f / det;

            //calculate distance from p1 to ray origin
            t = ray.origin - p1;

            //Calculate u parameter
            u = Vector3.Dot(t, p) * invDet;

            //Check for ray hit
            if (u < 0 || u > 1) { return false; }

            //Prepare to test v parameter
            q = Vector3.Cross(t, e1);

            //Calculate v parameter
            v = Vector3.Dot(ray.direction, q) * invDet;

            //Check for ray hit
            if (v < 0 || u + v > 1) { return false; }

            if ((Vector3.Dot(e2, q) * invDet) > Epsilon)
            {
                //ray does intersect
                return true;
            }

            // No hit at all
            return false;
        }
    }

}
