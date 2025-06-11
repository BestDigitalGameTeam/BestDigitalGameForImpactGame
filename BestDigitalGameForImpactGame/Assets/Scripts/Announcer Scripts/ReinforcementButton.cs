using UnityEngine;

public class ReinforcementButton : MonoBehaviour
{
    [SerializeField] private bool m_bIsPositive;
    [SerializeField] private bool m_bIsActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.ActivateReinforcementButtons.AddListener(Activate);
        GameManager.Instance.PlayerPressedReinforcementButton.AddListener(DeActivate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_bIsActive)
        {
            m_bIsActive = false;
            GameManager.Instance.PlayerPressedReinforcementButton.Invoke(m_bIsPositive);
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
