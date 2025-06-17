using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int m_iDamage = 10;
    [SerializeField] private float m_fLifetime = 5.0f;

    private void Start()
    {
        Destroy(gameObject, m_fLifetime); // Destroy after a delay
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(m_iDamage);

        Destroy(gameObject); // Destroy after hitting anything
    }
}
