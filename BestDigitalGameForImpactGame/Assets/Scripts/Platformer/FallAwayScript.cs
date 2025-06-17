using System;
using UnityEngine;

public class FallAwayScript : MonoBehaviour
{
    public float m_fDelay = 1.0f;
    public bool m_bRespawn = false;
    public float m_fRespawnTimer = 0.0f;
    private bool m_bDisabled;

    private MeshRenderer m_renderer;
    private BoxCollider m_trigger;
    private BoxCollider m_collider;

    private void Start()
    {
        m_renderer = gameObject.GetComponent<MeshRenderer>();
        m_trigger = gameObject.GetComponents<BoxCollider>()[0];
        m_collider = gameObject.GetComponents<BoxCollider>()[1];
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Add destruction Animation?
            Invoke(nameof(Disable),m_fDelay);
        }
    }

    private void Disable()
    {
        m_bDisabled = true;
        m_renderer.enabled = false;
        m_collider.enabled = false;
        m_trigger.enabled = false;
        if(m_bRespawn) Invoke(nameof(Enable),m_fRespawnTimer);
    }

    public void Enable()
    {
        if (m_bDisabled)
        {
            m_bDisabled = false;
            m_renderer.enabled = true;
            m_collider.enabled = true;
            m_trigger.enabled = true;
        }
    }
}
