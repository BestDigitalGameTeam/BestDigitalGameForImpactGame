using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int m_iMaxHealth = 100;
    private int m_iCurrentHealth;

    private void Start()
    {
        m_iCurrentHealth = m_iMaxHealth;
    }

    public void TakeDamage(int _iDamage)
    {
        m_iCurrentHealth -= _iDamage;
        Debug.Log("Player HP: " + m_iCurrentHealth);

        if (m_iCurrentHealth <= 0)
        {
            Debug.Log("Player died.");
            // Handle player death
        }
    }
}