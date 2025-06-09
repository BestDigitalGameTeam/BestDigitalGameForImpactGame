using UnityEngine;

public class PressurePad : MonoBehaviour
{
    [SerializeField] private Gate gate;  // Reference to gate script
    private int m_iObjectsOnPad = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.attachedRigidbody != null)
        {
            m_iObjectsOnPad++;
            gate.Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.attachedRigidbody != null)
        {
            m_iObjectsOnPad = Mathf.Max(0, m_iObjectsOnPad - 1);
            if (m_iObjectsOnPad == 0)
                gate.Close();
        }
    }
}