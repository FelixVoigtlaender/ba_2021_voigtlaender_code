using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SaveElement
{
    [SerializeField] public string nameElement;
    public event Action OnDelete;
    [SerializeField] private int id;
    [SerializeField] public bool isRoot = false;
    [SerializeField] public bool isDummy = false;
    [SerializeField] public Vector3 position = Vector3.zero;
    public SaveElement()
    {
        nameElement = this.ToString();
        id = SaveManager.GenerateID();
        SaveManager.AddSaveElement(this);
    }


    public virtual void Delete()
    {
        SaveManager.RemoveSaveElement(this);
        OnDelete?.Invoke();

    }
}
