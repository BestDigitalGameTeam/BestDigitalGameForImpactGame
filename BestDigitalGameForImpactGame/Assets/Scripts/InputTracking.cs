using UnityEngine;
using System.IO;

public class InputTracking : MonoBehaviour
{
    private string logFilePath;

    private void Start()
    {
        logFilePath = Path.GetFullPath(Path.Combine(Application.dataPath, "..")); // gets project path, one up 
        logFilePath = Path.Combine(logFilePath, "actionLog.txt");
        File.WriteAllText(logFilePath, "Input Log\n"); // overwrites previous text if it exists
        Debug.Log("Logging to " + logFilePath);
    }

    // Update is called once per frame
    private void Update()
    {
        // detect key press
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                LogKey(key.ToString());
            }
        }

        // detect mouse
        if (Input.GetMouseButtonDown(0)) LogKey("LeftClick");
        if (Input.GetMouseButtonDown(1)) LogKey("RightClick");
        if (Input.GetMouseButtonDown(2)) LogKey("MiddleClick");
    }

    private void LogKey(string button)
    {
        string logEntry = Time.time.ToString("F2") + "s: " + button; // gets time key is pressed, to 5 decimal places
        File.AppendAllText(logFilePath, logEntry + "\n"); // appends to file
        Debug.Log(logEntry);
    }
}
