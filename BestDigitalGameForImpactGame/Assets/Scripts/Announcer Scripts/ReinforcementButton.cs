using UnityEngine;

public class ReinforcementButton : MonoBehaviour
{
    [SerializeField] private bool m_bIsPositive;
    [SerializeField] private bool m_bIsActive;
    [SerializeField] private bool m_bTriggered;
    [SerializeField] private float m_fAnimationSpeed = 0.01f;
    [SerializeField] private Transform m_EndTrans;
    [SerializeField] private GameObject m_MovingObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.ActivateReinforcementButtons.AddListener(Activate);
        GameManager.Instance.PlayerPressedReinforcementButton.AddListener(DeActivate);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bTriggered && m_MovingObject.transform.position.y > m_EndTrans.position.y)
        {
            m_MovingObject.transform.position = new Vector3(m_MovingObject.transform.position.x,m_MovingObject.transform.position.y-m_fAnimationSpeed*Time.deltaTime,m_MovingObject.transform.position.z);
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
