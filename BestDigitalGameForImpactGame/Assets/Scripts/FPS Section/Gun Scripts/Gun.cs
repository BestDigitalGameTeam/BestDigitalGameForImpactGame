using UnityEngine;
using System.Collections;

// Handles gun behavior: shooting, reloading, and audio
public class Gun : MonoBehaviour
{
    [SerializeField] protected GunData gunData;                         // ScriptableObject holding gun stats and state
    [SerializeField] protected GameObject projectilePrefab;             // Prefab to instantiate when shooting
    [SerializeField] protected Transform projectileSpawnPoint;          // Where the projectile spawns from
    public AudioSource m_shootAudioPlayer;                    // Audio source for shooting sound
    [SerializeField] protected AudioClip m_ShootAudio;

    protected float m_fTimeSinceLastShot;                       // Timer to manage fire rate

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

    // Can be used to play effects
    protected void OnGunShot()
    {
        // Plays shooting sound with random pitch for variation
        m_shootAudioPlayer.pitch = Random.Range(0.85f, 1.15f);
        m_shootAudioPlayer.PlayOneShot(m_ShootAudio, GameManager.Instance.MasterVolume * GameManager.Instance.EffectsVolume);
        // ---
    }
    // ---
}
// ---