using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DebugLogToFile : MonoBehaviour
{
    private string filename = "";

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }
    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }
    // Start is called before the first frame update
    void Start()
    {
        filename = Application.dataPath + "/LogFile.text";
    }

    // Update is called once per frame
    public void Log(string logString, string stackTrace, LogType type) /* for test */
    {
        // TextWriter tw = new StreamWriter(filename, true);

        // tw.WriteLine(logString);

        // tw.Close();
    }
}
