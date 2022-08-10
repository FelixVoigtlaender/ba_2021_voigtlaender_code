using System;
using System.Collections;
using System.Collections.Generic;
using Google.MaterialDesign.Icons;
using UnityEngine;

[RequireComponent(typeof(MaterialIcon))]
public class BlinkMaterialIcon : MonoBehaviour
{
    private MaterialIcon materialIcon;
    private void Awake()
    {
        materialIcon = GetComponent<MaterialIcon>();
    }

    private void OnEnable()
    {
        StartCoroutine(Blink());
    }
    
    
    IEnumerator Blink()
    {
        while (true)
        {
            materialIcon.enabled = false;
            yield return new WaitForSeconds(0.2f);
            materialIcon.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
