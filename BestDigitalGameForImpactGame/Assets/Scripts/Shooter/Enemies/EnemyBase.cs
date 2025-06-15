using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Common Settings")]
    [SerializeField] protected float m_fAttackRange = 2f;       // Distance required to attack
    [SerializeField] protected float m_fAttackCooldown = 1.5f;  // Time between attacks
    [SerializeField] protected int m_iDamage = 10;              // Damage dealt to the player
    [SerializeField] protected int m_iMaxHealth = 100;          // Enemy's max health

    protected Transform m_PlayerTransform;      // Cached reference to player
    protected NavMeshAgent m_NavAgent;          // Handles pathfinding
    protected float m_fAttackTimer;             // Cooldown countdown
    protected int m_iCurrentHealth;             // Current health value

    // Called when enemy is created
    protected virtual void Start()
    {
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        m_iCurrentHealth = m_iMaxHealth;
    }
    // ---

    // Handles movement and attack range checks
    protected virtual void Update()
    {
        if (!m_PlayerTransform) return;

        float fDist = Vector3.Distance(transform.position, m_PlayerTransform.position);

        // Move toward player if out of attack range
        if (fDist > m_fAttackRange)
        {
            m_NavAgent.isStopped = false;
            m_NavAgent.SetDestination(m_PlayerTransform.position);
        }
        // ---
        
        else // In range to attack
        {
            m_NavAgent.isStopped = true;
            TryAttack();
        }

        m_fAttackTimer -= Time.deltaTime;
    }
    // ---

    // Implemented by child classes to define specific attack behavior
    protected abstract void TryAttack();

    // Called when the enemy receives damage
    public virtual void TakeDamage(int _iDamage)
    {
        m_iCurrentHealth -= _iDamage;

        if (m_iCurrentHealth <= 0)
            Die();
    }
    // ---

    // Handles enemy death
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        Destroy(gameObject); // Remove from scene
    }
    // ---
}
