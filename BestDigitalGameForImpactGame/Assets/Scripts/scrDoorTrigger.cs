using UnityEngine;

public class scrDoorTrigger : MonoBehaviour
{
    public string doorColor = "Red"; // Set this in Inspector
    public InputTracking inputLogger;

    private void Start()
    {
        inputLogger = FindObjectOfType<InputTracking>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && inputLogger != null)
        {
            inputLogger.LogEvent("Entered " + doorColor + " door");
        }
    }
}
