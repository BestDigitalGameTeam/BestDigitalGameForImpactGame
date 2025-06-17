using UnityEngine;
using UnityEngine.AI;

// Enumeration for enemy behavior states
public enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}

// Base enemy behavior script
public class EnemyBase : MonoBehaviour
{
    [Header("Combat")] [SerializeField] protected float m_fMaxHealth = 100.0f;  // Total health
    [SerializeField] protected float m_fAttackRange = 2.0f;                     // Distance required to attack
    [SerializeField] protected float m_fAttackCooldown = 1.5f;                  // Time between attacks
    [SerializeField] protected float m_fVisionRange = 20.0f;                    // How far the enemy can see
    [SerializeField] protected float m_fVisionAngle = 60.0f;                    // Cone angle for vision
    [SerializeField] protected float m_fDamage = 10.0f;                         // Damage dealt to the player

    [Header("Idle Wandering")] [SerializeField]
    private float m_fIdleWanderRadius = 5.0f;                                   // How far to wander during idle

    [SerializeField] private float m_fIdleWaitTime = 3.0f;                      // Wait time before picking next spot

    private float m_fIdleTimer;                                                 // Countdown for next idle movement

    protected float m_fCurrentHealth;                                           // Current HP
    protected Transform m_PlayerTransform;                                      // Reference to the player
    protected NavMeshAgent m_Agent;                                             // Pathfinding agent
    protected float m_fAttackTimer;                                             // Cooldown timer
    protected EnemyState m_State = EnemyState.Idle;                             // Current state
    
    [Header("Materials")]
    [SerializeField] private Material m_MaterialNormal;
    [SerializeField] private Material m_MaterialAlert;
    [SerializeField] private Material m_MaterialAttack;

    private Renderer m_Renderer;
    private bool m_bPlayerVisible = false;


    // Initialization
    protected virtual void Start()
    {
        m_fCurrentHealth = m_fMaxHealth;
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        m_Agent = GetComponent<NavMeshAgent>();
        
        m_Renderer = GetComponentInChildren<Renderer>();
        if (m_Renderer && m_MaterialNormal)
            m_Renderer.material = m_MaterialNormal;
    }
    // ---

    // Called every frame
    protected virtual void Update()
    {
        m_fAttackTimer -= Time.deltaTime;

        // State machine logic
        switch (m_State)
        {
            case EnemyState.Idle:
                if (m_Renderer && m_MaterialAlert)
                    m_Renderer.material = m_MaterialNormal;
                IdleWander(); // Wander randomly
                LookForPlayer(); // Check for player
                break;

            case EnemyState.Chasing:
                if (m_Renderer && m_MaterialAlert)
                    m_Renderer.material = m_MaterialAlert;
                MoveToPlayer(); // Move toward player
                break;

            case EnemyState.Attacking:
                if (m_Renderer && m_MaterialAlert)
                    m_Renderer.material = m_MaterialAttack;
                TryAttack(); // Try to attack
                break;
        }
    }
    // ---

    // Handles random wandering during idle state
    private void IdleWander()
    {
        m_fIdleTimer -= Time.deltaTime;

        // If not currently moving and timer expired
        if (!m_Agent.pathPending && m_Agent.remainingDistance <= m_Agent.stoppingDistance)
        {
            if (!(m_fIdleTimer <= 0f)) return;
            Vector3 v3NewPos;
            // Try to find a random reachable point on the NavMesh
            if (GetRandomNavMeshLocation(transform.position, m_fIdleWanderRadius, out v3NewPos))
            {
                m_Agent.SetDestination(v3NewPos);
                m_fIdleTimer = m_fIdleWaitTime;
            }
            // ---
        }
        // ---
    }
    // ---

    // Detects the player if within the enemy's vision cone
    protected void LookForPlayer()
    {
        if (!m_PlayerTransform) return;

        Vector3 v3ToPlayer = m_PlayerTransform.position - transform.position;
        float fAngle = Vector3.Angle(transform.forward, v3ToPlayer);
        float fDistance = v3ToPlayer.magnitude;

        // Check if player is within vision cone
        if (fDistance <= m_fVisionRange && fAngle <= m_fVisionAngle / 2.0f)
        {
            m_State = EnemyState.Chasing;
        }
    }

    // Moves the enemy toward the player
    protected void MoveToPlayer()
    {
        if (!m_PlayerTransform) return;

        m_Agent.SetDestination(m_PlayerTransform.position);

        // If close enough to attack, switch states
        float fDistance = Vector3.Distance(transform.position, m_PlayerTransform.position);
        if (!(fDistance <= m_fAttackRange)) return;
        m_Agent.ResetPath();
        m_State = EnemyState.Attacking;
        // ---
    }

    // Handles enemy attack logic
    protected virtual void TryAttack()
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

        // If cooldown expired, perform attack
        if (!(m_fAttackTimer <= 0f)) return;
        Debug.Log("Enemy attacks player!");
        m_fAttackTimer = m_fAttackCooldown;
        // Apply damage to player here
        // ---
    }

    // Applies damage to the enemy
    public virtual void TakeDamage(float _fDamage)
    {
        m_fCurrentHealth -= _fDamage;

        if (m_fCurrentHealth <= 0)
            Die();
        else
            m_State = EnemyState.Chasing; // Respond aggressively
    }
    // ---

    // Handles death behavior
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
    // ---

    // Finds a random valid position on the NavMesh
    private bool GetRandomNavMeshLocation(Vector3 _origin, float _radius, out Vector3 _result)
    {
        for (int i = 0; i < 10; i++) // Try up to 10 random points
        {
            Vector3 v3Random = _origin + Random.insideUnitSphere * _radius;

            // Check if position is on NavMesh
            if (!NavMesh.SamplePosition(v3Random, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) continue;
            _result = hit.position;
            return true;
            // ---
        }

        // Fallback: stay at current position
        _result = _origin;
        return false;
    }
    // ---

    private void OnDrawGizmosSelected()
    {
        // Draw vision radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_fVisionRange);
        // ---

        // Draw vision angle boundaries
        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -m_fVisionAngle / 2f, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, m_fVisionAngle / 2f, 0) * forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * m_fVisionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * m_fVisionRange);
        // ---

#if UNITY_EDITOR
        // Show player detection line if player exists
        if (Application.isPlaying && m_PlayerTransform)
        {
            Vector3 v3ToPlayer = m_PlayerTransform.position - transform.position;
            float fAngle = Vector3.Angle(transform.forward, v3ToPlayer);
            float fDistance = v3ToPlayer.magnitude;

            // Check if within cone
            bool bInVision = fDistance <= m_fVisionRange && fAngle <= m_fVisionAngle / 2f;

            Gizmos.color = bInVision ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, m_PlayerTransform.position);
            // ---

            // Draw a label
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 2,
                bInVision ? "Player Detected" : "Player Not Detected"
            );
            // ---
        }
        // ---
#endif
    }
}