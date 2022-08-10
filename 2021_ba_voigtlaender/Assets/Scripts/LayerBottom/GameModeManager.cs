using LayerSave;
using LayerTop;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace LayerBottom
{
    public class GameModeManager : MonoBehaviour
    {
        public Transform playerTransform;
        public GameObject locomotionSystem;
        private fvInputModeManager.Mode playMode;
    

        public bool isPlaying = false;

        private GameObjectReference xrRigReference;
    
    
        private void Start()
        {
            playMode = fvInputModeManager.instance.FindMode("PLAY");
            fvInputModeManager.instance.OnModeChanged += OnModeChanged;

            XRRig xrRig = FindObjectOfType<XRRig>();
            xrRigReference = new GameObjectReference(xrRig.gameObject);
            locomotionSystem.SetActive(false);
        }

        public void SaveObjects()
        {
            if(SaveManager.instance.programm == null)
                return;
        
            // Save Objects position before game start
            SaveManager.instance.programm.SaveObjects();
            // Save Editor rig before start
            xrRigReference.Save();
        
            // Set XRRig to player position
            xrRigReference.gameObject.transform.position = playerTransform.position;
            xrRigReference.gameObject.transform.localScale = playerTransform.localScale;
            xrRigReference.gameObject.transform.rotation = playerTransform.rotation;
            // Disable player reference
            playerTransform.gameObject.SetActive(false);
            // Enable game locomotion 
            locomotionSystem.SetActive(true);
            SetRayMode(XRRayInteractor.LineType.ProjectileCurve);
            VisManager.instance.SetVisibility(false);

            VRManager.tickIndex = 0;
            SaveManager.instance.programm.Start(new DatEvent(VRManager.tickIndex));

        }

        private void FixedUpdate()
        {
            if(!playMode.isActive)
                return;

            // For Interaction to work
            playerTransform.position = xrRigReference.gameObject.transform.position;
        }

        void SetRayMode(XRRayInteractor.LineType lineType)
        {
            XRRayInteractor[] rayInteractors = FindObjectsOfType<XRRayInteractor>();
            foreach (var rayInteractor in rayInteractors)
            {
                rayInteractor.lineType = lineType;
            }
        }

        public void LoadObjects()
        {
        
            if(SaveManager.instance.programm == null)
                return;
        
            SaveManager.instance.programm.LoadObjects();
            xrRigReference.Load();
            playerTransform.gameObject.SetActive(true);
            locomotionSystem.SetActive(false);
            VisManager.instance.SetVisibility(true);
            SetRayMode(XRRayInteractor.LineType.StraightLine);
        }
    
        public void OnModeChanged(fvInputModeManager.Mode mode)
        {

            if (mode == playMode)
            {
                isPlaying = true;
                SaveObjects();
            }
            else
            {
                if (isPlaying)
                {
                    isPlaying = false;
                    LoadObjects();
                }
            }
        
        }


    }
}
