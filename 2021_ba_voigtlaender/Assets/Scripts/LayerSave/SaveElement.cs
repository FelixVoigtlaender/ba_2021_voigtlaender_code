using System;
using LayerBottom;
using UnityEngine;

namespace LayerSave
{
    [System.Serializable]
    public class SaveElement
    {
        [SerializeField] public string nameElement;
        public event Action OnDelete;
        public event Action OnSave;
        [SerializeField] public bool isRoot = false;
        [SerializeField] public bool isDummy = false;
        [SerializeField] public Vector3 position = Vector3.zero;


        public virtual void Save()
        {
            nameElement = this.ToString();
            SaveManager.AddSaveElement(this);
        
            OnSave?.Invoke();
        }

        public virtual void Delete()
        {
            SaveManager.RemoveSaveElement(this);
            OnDelete?.Invoke();
        }
    
    
        public virtual void Update(DatEvent datEvent)
        {
        
        }

        public virtual void FixedUpdate(DatEvent datEvent)
        {
        
        }
        public virtual void Start(DatEvent datEvent)
        {
        
        }
    }
}
