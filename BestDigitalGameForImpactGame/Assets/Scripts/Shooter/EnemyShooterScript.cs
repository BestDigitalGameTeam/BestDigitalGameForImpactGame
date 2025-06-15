using System;
using UnityEngine;

public class EnemyShooterScript : MonoBehaviour
{
    private float m_fLastShotTime = 0.0f;
    public float m_fShootRate = 1.0f;
    public float m_fProjectileSpeed = 5.0f;
    private Transform m_AIPos;

    [SerializeField] private GameObject m_EnemyProjPrefab;
    [SerializeField] private Transform m_ShootPoint;

    private void Start()
    {
        m_AIPos = FindObjectsByType<FPSAIController>(FindObjectsSortMode.None)[0].gameObject.transform; //Should only ever be one FPSAiController
    }

    void Shoot()
    {
        m_ShootPoint.LookAt(m_AIPos);
        GameObject projectile = Instantiate(m_EnemyProjPrefab,m_ShootPoint.position, m_ShootPoint.rotation);
        projectile.GetComponent<Rigidbody>().linearVelocity = m_ShootPoint.forward * m_fProjectileSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_fLastShotTime + m_fShootRate <= Time.time)
        {
            Shoot();
            m_fLastShotTime = Time.time;
        }
    }
}
