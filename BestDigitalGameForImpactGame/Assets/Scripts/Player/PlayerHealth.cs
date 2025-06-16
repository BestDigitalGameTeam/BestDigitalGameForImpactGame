using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int m_iMaxHealth = 100;
    private float m_fCurrentHealth;

    private void Start()
    {
        m_fCurrentHealth = m_iMaxHealth;
    }

    public void TakeDamage(float _fDamage)
    {
        m_fCurrentHealth -= _fDamage;
        Debug.Log("Player HP: " + m_fCurrentHealth);

        if (m_fCurrentHealth <= 0)
        {
            Debug.Log("Player died.");
            // Handle player death
            // Load death hud
            // reload level
        }
    }
}