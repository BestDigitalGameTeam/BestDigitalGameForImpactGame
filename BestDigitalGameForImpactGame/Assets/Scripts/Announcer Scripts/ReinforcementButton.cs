using UnityEngine;

public class ReinforcementButton : MonoBehaviour
{
    [SerializeField] private bool m_bIsPositive;
    [SerializeField] private bool m_bIsActive = false;
    [SerializeField] private Animator m_Animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.ActivateReinforcementButtons.AddListener(Activate);
        GameManager.Instance.PlayerPressedReinforcementButton.AddListener(DeActivate);
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_bIsActive || other.gameObject.CompareTag("Player"))
        {
            m_bIsActive = false;
            GameManager.Instance.PlayerPressedReinforcementButton.Invoke(m_bIsPositive);
            m_Animator.SetTrigger("killme");
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
