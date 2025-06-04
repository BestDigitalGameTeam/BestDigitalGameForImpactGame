using UnityEngine;

// File authour: Charli

public class TriggerAnnouncerBiasEvent : MonoBehaviour
{
    private Collider m_TriggerArea;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_TriggerArea = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider _other)
    {
        AnnouncerAlgorithm.Instance.CalculateBiasWeightings();
    }
}
