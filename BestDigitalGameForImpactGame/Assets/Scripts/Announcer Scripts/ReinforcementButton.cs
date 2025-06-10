using UnityEngine;

public class ReinforcementButton : MonoBehaviour
{
    [SerializeField] private bool m_bIsPositive;
    private bool m_bIsActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.ActivateReinforcementButtons.AddListener(Activate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_bIsActive)
        {
            GameManager.Instance.PlayerPressedReinforcementButton.Invoke(m_bIsPositive);
            m_bIsActive = false;
        }
    }

    private void Activate()
    {
        m_bIsActive = true;
    }
}
