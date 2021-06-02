using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRLogicElement : MonoBehaviour
{
    public List<VRPort> inputs;
    public List<VRPort> outputs;


    private void Awake()
    {
        SetupInputs();
        SetupOutputs();
    }

    public virtual void SetupInputs()
    {
        inputs = new List<VRPort>();
    }

    public virtual void SetupOutputs()
    {
        outputs = new List<VRPort>();
    }

    public virtual VRData GetData()
    {
        return null;
    }
    public virtual void Trigger()
    {

    }



}
