using UnityEngine;

public class EnemyMelee : EnemyBase
{
    // Melee attack logic
    protected override void TryAttack()
    {
        if (m_fAttackTimer <= 0f)
        {
            Debug.Log("Melee Enemy attacks!");

            // Apply damage to player
            m_PlayerTransform.GetComponent<PlayerHealth>()?.TakeDamage(m_iDamage);

            // Reset attack cooldown
            m_fAttackTimer = m_fAttackCooldown;
        }
    }
    // ---
}