using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LayerBottom
{
    [System.Serializable]
    public abstract class VRData 
    {
        public abstract bool IsType(VRData data);
        public abstract Color GetColor();
        public abstract string GetName();
        public abstract void SetData(VRData data);

        public event Action<VRData> OnDataChanged;
        protected void DataChanged()
        {
            OnDataChanged?.Invoke(this);
        }
        public Color DecimalToColor(int r, int g, int b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }
    }

    [System.Serializable]
    public class DatString : VRData
    {
        [SerializeField]private string value;
        public string Value 
        {
            get { return value; }
            set { this.value = value; DataChanged(); }
        }

        public DatString(string value)
        {
            this.Value = value;
        }


        public override bool IsType(VRData data)
        {
            return data is DatString;
        }
        public override Color GetColor()
        {
            //Magenta
            return DecimalToColor(255, 0, 255);
        }

        public override string GetName()
        {
            return Value.ToString();
        }

        public override void SetData(VRData data)
        {
            if (data == null)
                return;
            Value = ((DatString)data).Value;
        }
    }

    [System.Serializable]
    public class DatFloat : VRData
    {
        [SerializeField]private float value;
        public float Value
        {
            get { return value; }
            set { this.value = value; DataChanged(); }
        }
        public bool useMinMax = true;
        public float min = 0;
        public float max = 10;

        public DatFloat(float value)
        {
            this.Value = value;
        }
        public override bool IsType(VRData data)
        {
            return data is DatFloat;
        }
        public override Color GetColor()
        {
            //Yellow green
            return DecimalToColor(154, 205, 50);
        }
        public override string GetName()
        {
            return Value.ToString("0.00");
        }
        public override void SetData(VRData data)
        {
            if (data == null)
                return;
            Value = ((DatFloat)data).Value;
        }
    }


    [System.Serializable]
    public class DatColor : VRData
    {
        [SerializeField]private Color value;
        public Color Value
        {
            get { return value; }
            set { this.value = value; DataChanged(); }
        }

        public DatColor(Color value)
        {
            this.Value = value;
        }
        public override bool IsType(VRData data)
        {
            return data is DatColor;
        }
        public override Color GetColor()
        {
            //purple
            return DecimalToColor(128, 0, 128);
        }
        public override string GetName()
        {
            return "";
        }
        public override void SetData(VRData data)
        {
            if (data == null)
                return;
            Value = ((DatColor)data).Value;
        }
    }


    [System.Serializable]
    public class DatInt : VRData
    {
        [SerializeField]private int value;
        public int Value
        {
            get { return value; }
            set { this.value = value; DataChanged(); }
        }
        public DatInt(int value)
        {
            this.Value = value;
        }
        public override bool IsType(VRData data)
        {
            return data is DatInt;
        }
        public override Color GetColor()
        {
            //Sea green
            return DecimalToColor(46, 139, 87);
        }
        public override string GetName()
        {
            return Value.ToString();
        }
        public override void SetData(VRData data)
        {
            if (data == null)
                return;
            Value = ((DatInt)data).Value;
        }
    }


    [System.Serializable]
    public class DatVector3 : VRData
    {
        [SerializeField]private Vector3 value;
        public Vector3 Value
        {
            get { return value; }
            set { this.value = value; DataChanged(); }
        }
        public DatVector3(Vector3 value)
        {
            this.Value = value;
        }
        public override bool IsType(VRData data)
        {
            return data is DatVector3;
        }
        public override Color GetColor()
        {
            //Gold
            return DecimalToColor(255, 215, 0);
        }
        public override string GetName()
        {
            return Value.ToString();
        }
        public override void SetData(VRData data)
        {
            if (data == null)
                return;
            Value = ((DatVector3)data).Value;
        }
    }


    [System.Serializable]
    public class DatQuaternion : VRData
    {
        [SerializeField]private Quaternion value;
        public Quaternion Value
        {
            get { return value; }
            set { this.value = value; DataChanged(); }
        }
        public DatQuaternion(Quaternion value)
        {
            this.Value = value;
        }
        public override bool IsType(VRData data)
        {
            return data is DatQuaternion;
        }
        public override Color GetColor()
        {
            // Violett
            return DecimalToColor(153, 50, 204);
        }
        public override string GetName()
        {
            return Value.ToString();
        }
        public override void SetData(VRData data)
        {
            if (data == null)
                return;
            Value = ((DatQuaternion)data).Value;
        }
    }

    [System.Serializable]
    public class DatEvent : VRData
    {
        [SerializeField]private int value;
        public int Value
        {
            get { return value; }
            set { this.value = value; DataChanged(); }
        }
        public DatEvent(int value)
        {
            this.Value = value;
        }
        public override bool IsType(VRData data)
        {
            return data is DatEvent;
        }
        public override Color GetColor()
        {
            
            //Yellow
            return DecimalToColor(195, 166, 23);
            
            //White
            //return DecimalToColor(255, 255, 255);
        }
        public override string GetName()
        {
            return "Event: " + Value.ToString();
        }
        public override void SetData(VRData data)
        {
            if (data == null)
                return;
            Value = ((DatEvent)data).Value;
        }
    }

    [System.Serializable]
    public class DatBool : VRData
    {
        [SerializeField] private bool value;
        public bool Value
        {
            get { return value; }
            set { this.value = value; DataChanged(); }
        }
        public DatBool(bool value)
        {
            this.Value = value;
        }
        public override bool IsType(VRData data)
        {
            return data is DatBool;
        }
        public override Color GetColor()
        {
            //Maroon
            return DecimalToColor(128, 0, 0);
        }
        public override string GetName()
        {
            return Value.ToString();
        }
        public override void SetData(VRData data)
        {
            if (data == null)
                return;
            Value = ((DatBool)data).Value;
        }
    }

    [System.Serializable]
    public class DatObj : VRData
    {
        [SerializeReference] private VRObject value;
        public VRObject Value
        {
            get { return value; }
            set { this.value = value; DataChanged(); }
        }
        public DatObj(VRObject value)
        {
            this.Value = value;
        }
        public override bool IsType(VRData data)
        {
            return data is DatObj;
        }
        public override Color GetColor()
        {
            //Maroon
            return DecimalToColor(0, 0, 0);
        }
        public override string GetName()
        {
            if (Value == null)
                return "_";
            if (Value.gameObject == null)
                return "_.";
            return Value.gameObject.name;
        }
        public override void SetData(VRData data)
        {
            if (data == null)
                return;
            Value = ((DatObj)data).Value;
        }
    }

    [System.Serializable]
    public class DatTransform : VRData
    {
        [SerializeReference] public DatObj datObj;
        [SerializeReference] public DatVector3 datPosition;
        [SerializeReference] public DatQuaternion datRotation;
        [SerializeReference] public DatVector3 datLocalScale;
        public DatTransform(DatObj datObj, DatVector3 datPosition, DatQuaternion datRotation, DatVector3 datLocalScale)
        {
            this.datPosition = datPosition;
            this.datRotation = datRotation;
            this.datLocalScale = datLocalScale;
            this.datObj = datObj;

            datPosition.OnDataChanged += (value) => DataChanged();
            datRotation.OnDataChanged += (value) => DataChanged();
            datLocalScale.OnDataChanged += (value) => DataChanged();
            datObj.OnDataChanged += (value) => DataChanged();
        }
        public DatTransform(DatObj datObj, Vector3 position, Quaternion rotation, Vector3 localScale) :
            this(datObj,new DatVector3(position), new DatQuaternion(rotation), new DatVector3(localScale)) {}
        public DatTransform(DatObj datObj) :
            this(datObj,datObj.Value.gameObject.transform.position, datObj.Value.gameObject.transform.rotation, datObj.Value.gameObject.transform.localScale)
        { }

        public override bool IsType(VRData data)
        {
            return data is DatTransform;
        }
        public override Color GetColor()
        {
            //deepskyblue
            return DecimalToColor(0, 191, 255);
        }
        public override string GetName()
        {
            return "Transform";
        }
        public override void SetData(VRData data)
        {
            if (data == null)
                return;
            DatTransform datTransform = (DatTransform)data;
            datPosition.SetData(datTransform.datPosition);
            datRotation.SetData(datTransform.datRotation);
            datLocalScale.SetData(datTransform.datLocalScale);
        }
    }


    [System.Serializable]
    public class DatRecording : VRData
    {
        [SerializeReference] public DatTransform datTransform;
        [SerializeReference] List<DatTransform> value;
        public List<DatTransform> Value
        {
            get { return value; }
            set { this.value = value; DataChanged(); }
        }
        public DatRecording(DatTransform datTransform, List<DatTransform> value)
        {
            this.datTransform = datTransform;
            Value = value;
        }
        public DatRecording(DatTransform datTransform) : this(datTransform, new List<DatTransform>()) { }

        public override bool IsType(VRData data)
        {
            return data is DatRecording;
        }
        public override Color GetColor()
        {
            //Apple Barrel Bright Blue
            return DecimalToColor(0, 153, 204);
        }
        public override string GetName()
        {
            return "Recording";
        }
        public override void SetData(VRData data)
        {
            if (data == null)
                return;
            DatRecording other = (DatRecording)data;
            Value = other.Value;
            //datTransform = other.datTransform;
        }
        public void AddDatTransform(DatTransform datTransform)
        {
            value.Add(datTransform);
            DataChanged();
        }
    }
}