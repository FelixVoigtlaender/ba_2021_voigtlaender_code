using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRDebug : MonoBehaviour
{
    static VRDebug instance;
    TMPro.TMP_Text textField;

    string logText;
    string setLogText;
    static int logCount;
    public void Awake()
    {
        textField = GetComponent<TMPro.TMP_Text>();
        instance = this;
    }

    public void LateUpdate()
    {
        string setColor = "<color=#FF665A>";
        string logColor = "<color=#FF8C64>";
        string text = $"{setColor} {setLogText} \n {logColor} {logText}";
        textField.text = text;

        setLogText = "";
    }


    public static void Log(string message)
    {
        if (!instance)
            return;
        string conMessage = logCount + ": " + message + "\n";
        instance.logText += conMessage;
        logCount++;
        instance.logText = CutLines(instance.logText, 15);
        Debug.Log(conMessage);
    }

    public static string CutLines(string message, int lineCount)
    {
        string[] lines = message.Split('\n');
        int startIndex = lines.Length - Mathf.Min(lineCount, lines.Length);
        string cutMessage = "";
        for(int i = startIndex; i < lines.Length; i++)
        {
            if (lines[i].Length <= 1)
                continue;
            cutMessage += lines[i] + "\n";
        }

        return cutMessage;
    }
    public static void SetLog(string message)
    {
        //Debug.Log(message);
        if (!instance)
            return;

        instance.setLogText += message + "\n";
    }

}
