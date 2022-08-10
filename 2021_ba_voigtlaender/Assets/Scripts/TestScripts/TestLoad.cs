using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LayerBottom;

public class TestLoad : MonoBehaviour
{
    public TestData testData;
    public TestData testDataB;
    public string jsonString;
    private void Start()
    {
        testData = new TestData();
        testData.type = typeof(VRPort);

        jsonString = JsonUtility.ToJson(testData);
        print(jsonString);
        testDataB = (TestData) JsonUtility.FromJson(jsonString,typeof(TestData));
    }
    [System.Serializable]
    public class TestData 
    {
        public Type type;

    }

}
