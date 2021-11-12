using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



[RequireComponent(typeof(fvInputManager))]
public class InitLogic : MonoBehaviour
{
    [Header("Visuals")]
    public LoadingCircle loadingCircle;
    public float loadDelay = 0.5f;
    float loadStart = 0;
    LogicObject logicObject;

    [Header("Input")]
    public InputActionReference button;
    public string modeName = "";
    public string buttonText = "";
    fvInputManager inputManager;

    fvInputModeManager inputModeManager;
    fvInputModeManager.ButtonModeHandler handler;

    [Header("Raycast")]
    public LayerMask layerMask;
    public float maxDistance;

    private void Start()
    {
        inputManager = GetComponentInParent<fvInputManager>();
        inputModeManager = GetComponentInParent<fvInputModeManager>();

        handler = inputModeManager.AddButtonMode(button, buttonText, modeName);
        handler.OnButtonDown += OnButtonDown;
    }
    public void OnButtonDown(InputAction.CallbackContext context)
    {
        loadStart = Time.time;
        logicObject = null;


        if (inputManager.isUIHitClosest)
        {
            if (inputManager.uiRaycastHit.HasValue)
            {
                GameObject gameObject = inputManager.uiRaycastHit.Value.gameObject;
                Vector3 hitPoint = inputManager.uiRaycastHit.Value.worldPosition;
                Vector3 hitNormal = inputManager.uiRaycastHit.Value.worldNormal;
                logicObject = new LogicObject(gameObject, hitPoint, hitNormal);
            }
        }
        else
        {
            if (inputManager.worldRaycastHit.HasValue)
            {

                GameObject gameObject = inputManager.worldRaycastHit.Value.collider.gameObject;
                Vector3 hitPoint = inputManager.worldRaycastHit.Value.point;
                Vector3 hitNormal = inputManager.worldRaycastHit.Value.normal;
                logicObject = new LogicObject(gameObject, hitPoint, hitNormal);

            }
        }

        if (logicObject == null)
            return;
        if (logicObject.gameObject.GetComponentInParent<BlockCode>())
        {
            logicObject = null;
            return;
        }
    }

    public void Update()
    {
        if (!handler.isPressed || logicObject == null)
        {
            loadingCircle.alpha = 0;
            logicObject = null;
            return;
        }

        loadingCircle.transform.position = logicObject.hitPoint + logicObject.hitNormal*0.01f;
        loadingCircle.transform.forward = logicObject.hitNormal;

        loadingCircle.alpha = 1;
        float percent = (Time.time - loadStart) / loadDelay;
        loadingCircle.fillAmount = Mathf.Clamp01(percent);


        if(percent > 1){
            VRManager.instance.InitVRObject(logicObject.gameObject, loadingCircle.transform.position);
            logicObject = null;
        }

    }


    public class LogicObject
    {
        public GameObject gameObject;
        public Vector3 hitPoint;
        public Vector3 hitNormal;

        public LogicObject(GameObject gameObject, Vector3 hitPoint, Vector3 hitNormal)
        {
            this.gameObject = gameObject;
            this.hitPoint = hitPoint;
            this.hitNormal = hitNormal;
        }
    }
}
