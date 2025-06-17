using UnityEngine;

public class ReinforcementButton : MonoBehaviour
{
    [SerializeField] private bool m_bIsPositive;
    [SerializeField] private bool m_bIsActive;
    [SerializeField] private bool m_bTriggered;
    [SerializeField] private float m_fAnimationSpeed = 0.01f;
    [SerializeField] private Transform m_EndTrans;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.ActivateReinforcementButtons.AddListener(Activate);
        GameManager.Instance.PlayerPressedReinforcementButton.AddListener(DeActivate);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bTriggered && transform.position.y > m_EndTrans.position.y)
        {
            transform.position = new Vector3(transform.position.x,transform.position.y-(m_fAnimationSpeed*Time.deltaTime),transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_bIsActive && other.gameObject.CompareTag("Player"))
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
