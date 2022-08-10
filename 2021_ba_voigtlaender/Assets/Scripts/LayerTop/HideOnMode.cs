using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnMode : MonoBehaviour
{
    public string modeName = "PLAY";

    private fvInputModeManager.Mode mode;
    // Start is called before the first frame update
    void Start()
    {
         mode = fvInputModeManager.instance.FindMode(modeName);
         mode.onModeChange += OnModeChanged;
    }

    public void OnModeChanged(bool value)
    {
        gameObject.SetActive(!value);
    }

    private void OnDestroy()
    {
        if(mode!=null)
            mode.onModeChange -= OnModeChanged;
    }
}
