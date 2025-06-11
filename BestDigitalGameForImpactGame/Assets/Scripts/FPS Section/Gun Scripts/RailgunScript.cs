using UnityEngine;
using System.Collections;

// Handles gun behavior: shooting, reloading, and audio
public class RailgunScript : Gun
{
    private void Start()
    {
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
        // Only shoot if there is ammo and fire rate condition is met
        if (gunData.m_iCurrentAmmo <= 0 || !CanShoot()) return;
        
        // Instantiate projectile at spawn point and apply velocity
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        Rigidbody projectileRigidBody = projectile.GetComponent<Rigidbody>();
        projectileRigidBody.linearVelocity = projectileSpawnPoint.forward * gunData.m_fProjectileSpeed;
        // ---

        // Reduce ammo and reset shot timer
        --gunData.m_iCurrentAmmo;
        m_fTimeSinceLastShot = 0;
        // ---
        
        OnGunShot(); // Play Effects
    }

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