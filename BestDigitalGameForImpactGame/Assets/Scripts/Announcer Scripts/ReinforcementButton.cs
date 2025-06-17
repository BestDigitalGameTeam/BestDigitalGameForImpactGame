using UnityEngine;

public class ReinforcementButton : MonoBehaviour
{
    [SerializeField] private bool m_bIsPositive;
    [SerializeField] private bool m_bIsActive = false;
    [SerializeField] private bool m_bTriggered;
    private float m_fAnimationSpeed = 1.0f;
    private Transform m_EndTrans;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.ActivateReinforcementButtons.AddListener(Activate);
        GameManager.Instance.PlayerPressedReinforcementButton.AddListener(DeActivate);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bTriggered && transform.position != m_EndTrans.position)
        {
            transform.position += Vector3.Normalize(transform.position - m_EndTrans.position) * m_fAnimationSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_bIsActive || other.gameObject.CompareTag("Player"))
        {
            m_bIsActive = false;
            GameManager.Instance.PlayerPressedReinforcementButton.Invoke(m_bIsPositive);
            m_bTriggered = true;
        }
    }

    private void Activate()
    {
        m_bIsActive = true;
    }

    private void DeActivate(bool _b)
    {
        m_bIsActive = false;
    }
}
