using UnityEngine;

// Makes the pickup object float and rotate
public class PickupFloatSpin : MonoBehaviour
{
    [Header("Spin")]
    [SerializeField] private float m_fSpinSpeed = 45.0f; // Degrees per second

    [Header("Bob")]
    [SerializeField] private float m_fBobHeight = 0.25f; // How high it bobs
    [SerializeField] private float m_fBobSpeed = 2.0f;   // How fast it bobs

    private Vector3 m_v3StartPos;

    private void Start()
    {
        m_v3StartPos = transform.position;
    }

    private void Update()
    {
        // Spin around Y-axis
        transform.Rotate(Vector3.up * m_fSpinSpeed * Time.deltaTime, Space.World);

        // Bob up and down using sine wave
        float fOffset = Mathf.Sin(Time.time * m_fBobSpeed) * m_fBobHeight;
        transform.position = m_v3StartPos + Vector3.up * fOffset;
    }
}
// ---