using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private Vector3 m_v3OpenOffset = new Vector3(0, 5, 0);
    [SerializeField] private float m_fSpeed = 2.0f;

    private Vector3 m_v3ClosedPosition;
    private Vector3 m_v3OpenPosition;
    [SerializeField] private bool m_bIsOpening = false;

    private void Start()
    {
        m_v3ClosedPosition = transform.position;
        m_v3OpenPosition = m_v3ClosedPosition + m_v3OpenOffset;
    }

    public void Open()
    {
        m_bIsOpening = true;
    }

    public void Close()
    {
        m_bIsOpening = false;
    }

    private void Update()
    {
        Vector3 v3Target = m_bIsOpening ? m_v3OpenPosition : m_v3ClosedPosition;
        transform.position = Vector3.MoveTowards(transform.position, v3Target, m_fSpeed * Time.deltaTime);
    }
}