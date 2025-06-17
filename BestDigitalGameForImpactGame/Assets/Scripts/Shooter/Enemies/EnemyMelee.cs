using UnityEngine;

public class EnemyMelee : EnemyBase
{
    // Melee attack logic
    protected override void TryAttack()
    {
        if (!m_PlayerTransform) return;

        float fDistance = Vector3.Distance(transform.position, m_PlayerTransform.position);

        // If player moved out of range, chase again
        if (fDistance > m_fAttackRange)
        {
            m_State = EnemyState.Chasing;
            return;
        }
        // ---

        if (m_fAttackTimer <= 0f)
        {
            Debug.Log("Melee Enemy attacks!");

            // Apply damage to player
            m_PlayerTransform.GetComponent<PlayerHealth>()?.TakeDamage(m_fDamage);

            // Reset attack cooldown
            m_fAttackTimer = m_fAttackCooldown;
        }
    }
    // ---
}