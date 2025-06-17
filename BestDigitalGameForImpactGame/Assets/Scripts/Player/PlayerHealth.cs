using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int m_iMaxHealth = 100;
    private float m_fCurrentHealth;

    public UnityEvent<float> PlayerHealthChanged;

    private void Start()
    {
        m_fCurrentHealth = m_iMaxHealth;
    }

    public void TakeDamage(float _fDamage)
    {
        m_fCurrentHealth -= _fDamage;
        PlayerHealthChanged.Invoke(m_fCurrentHealth);
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