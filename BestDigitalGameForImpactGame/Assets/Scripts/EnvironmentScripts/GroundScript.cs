using UnityEngine;
using UnityEngine.Events;

// testing out using unity events
// hurrah
public class GroundScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AnnouncerAlgorithm.Instance.Announcer_Happy.AddListener(MakeGroundGreen);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MakeGroundGreen()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
    }
}
