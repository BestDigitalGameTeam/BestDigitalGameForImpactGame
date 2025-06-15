using System;
using UnityEngine;

// Handles projectile lifetime and collision
public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_flifeTime = 5.0f;  // Time before the projectile is automatically destroyed
    [SerializeField] private float m_fDamage;           // This is replacing the damage in the gun data script
    private float m_fSpawnTime;                         // Time when the projectile was spawned
    
    public void SetData(float _fDamage)
    {
        m_fDamage = _fDamage; // Called when projectile is instantiated
    }

    private void Start()
    {
        m_fSpawnTime = Time.time; // Record the time the projectile was created
    }

    private void Update()
    {
        // Destroy the projectile if it has existed longer than its lifetime
        if (m_fSpawnTime + m_flifeTime <= Time.time) Destroy(gameObject);
        // ---
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Hit: {collision.gameObject.name}"); // Log the name of the object hit
        
        // Damage enemy if hit
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBase pEnemy = collision.gameObject.GetComponent<EnemyBase>();
            if (pEnemy != null)
            {
                pEnemy.TakeDamage(m_fDamage);
            }
        }
        
        Destroy(gameObject); // Destroy the projectile upon collision
    }

}
// ---