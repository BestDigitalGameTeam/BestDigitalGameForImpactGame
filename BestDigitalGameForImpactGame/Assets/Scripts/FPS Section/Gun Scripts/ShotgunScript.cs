using UnityEngine;
using System.Collections;

// Handles gun behavior: shooting, reloading, and audio
public class ShotgunScript : Gun
{
    [SerializeField] int m_iPelletCount = 8;                     // Number of pellets per shot
    [SerializeField] float m_fSpreadAngle = 10.0f;               // Max angle of spread

    private void Start()
    {
        base.Start();
        PlayerShoot.ShootInput += Shoot; // Subscribe Shoot method to player shooting input
        
        gunData.m_iCurrentAmmo = gunData.m_iClipSize; // Fill ammo to clip size at start
    }

    private void Update()
    {
        m_fTimeSinceLastShot += Time.deltaTime; // Increment time since last shot

        // Check for reload input (R key), only reload if not full and not already reloading
        if (Input.GetKeyDown(KeyCode.R) && !gunData.m_bReloading && gunData.m_iCurrentAmmo < gunData.m_iClipSize)
            StartCoroutine(Reload());
        // ---
    }

    // Checks if gun can fire based on reload state and fire rate
    private bool CanShoot() => !gunData.m_bReloading && m_fTimeSinceLastShot >= 60.0f / gunData.m_fFireRate;

    // Called when player attempts to shoot
    private void Shoot()
    {
        if (gunData.m_iCurrentAmmo <= 0 || !CanShoot()) return;

        // Fire multiple pellets with random spread
        for (int i = 0; i < m_iPelletCount; ++i)
        {
            // Random direction within a cone
            Vector3 v3Spread = Quaternion.Euler(
                Random.Range(-m_fSpreadAngle, m_fSpreadAngle),
                Random.Range(-m_fSpreadAngle, m_fSpreadAngle),
                0) * projectileSpawnPoint.forward;
            // ---

            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.LookRotation(v3Spread));
            Rigidbody RigidBodyProjectile = projectile.GetComponent<Rigidbody>();
            RigidBodyProjectile.linearVelocity = v3Spread * gunData.m_fProjectileSpeed;
        }

        --gunData.m_iCurrentAmmo;
        m_fTimeSinceLastShot = 0;

        OnGunShot();
    }
    // ---

    // Coroutine to handle reloading delay and state
    private IEnumerator Reload()
    {
        Debug.Log("Reloading...");
        gunData.m_bReloading = true;

        // Wait for reload time
        yield return new WaitForSeconds(gunData.m_fReloadTime);

        // Restore full ammo and reset reloading state
        gunData.m_iCurrentAmmo = gunData.m_iClipSize;
        gunData.m_bReloading = false;
        Debug.Log("Reload complete.");
        // ---
    }
    // ---
}
// ---