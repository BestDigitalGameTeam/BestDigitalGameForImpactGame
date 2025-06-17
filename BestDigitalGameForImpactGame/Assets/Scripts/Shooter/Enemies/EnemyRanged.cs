using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Ranged Settings")]
    [SerializeField] private GameObject m_ProjectilePrefab;         // Projectile prefab to spawn
    [SerializeField] private Transform m_FirePoint;                 // Origin of the projectile
    [SerializeField] private float m_fProjectileSpeed = 15.0f;      // Speed of the projectile

    // Override attack behavior with ranged logic
    protected override void TryAttack()
    {
        if (m_fAttackTimer <= 0f)
        {
            FireProjectile();
            m_fAttackTimer = m_fAttackCooldown;
        }
    }
    // ---

    // Spawns and launches a projectile toward the player
    private void FireProjectile()
    {
        if (!m_ProjectilePrefab || !m_FirePoint || !m_PlayerTransform) return;

        GameObject pProjectile = Instantiate(m_ProjectilePrefab, m_FirePoint.position, m_FirePoint.rotation);
        Rigidbody pRigidBodyProjectile = pProjectile.GetComponent<Rigidbody>();
        pRigidBodyProjectile.linearVelocity = m_FirePoint.forward * m_fProjectileSpeed;

        Debug.Log("Ranged Enemy fires projectile.");
    }
    // ---
}