using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    [SerializeField] GunData gunData;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnPoint;
    public AudioSource m_shootAudioPlayer;

    private float m_fTimeSinceLastShot;

    private void Start()
    {
        PlayerShoot.ShootInput += Shoot;
        gunData.m_iCurrentAmmo = gunData.m_iClipSize;
    }

    private void Update()
    {
        m_fTimeSinceLastShot += Time.deltaTime;

        // Reload input
        if (Input.GetKeyDown(KeyCode.R) && !gunData.m_bReloading && gunData.m_iCurrentAmmo < gunData.m_iClipSize)
        {
            StartCoroutine(Reload());
        }
    }

    private bool CanShoot() => !gunData.m_bReloading && m_fTimeSinceLastShot >= 60.0f / gunData.m_fFireRate;

    public void Shoot()
    {
        if (gunData.m_iCurrentAmmo > 0 && CanShoot())
        {
            /*// Raycast Version - Will replace with projectile
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, gunData.m_fRange))
            {
                Debug.Log(hitInfo.transform.m_sWeaponName);
            }
            // ---*/
            
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            Rigidbody projectileRigidBody = projectile.GetComponent<Rigidbody>();
            projectileRigidBody.linearVelocity = projectileSpawnPoint.forward * gunData.m_fProjectileSpeed;

            --gunData.m_iCurrentAmmo;
            m_fTimeSinceLastShot = 0;
            OnGunShot();
        }
    }

    private IEnumerator Reload()
    {
        Debug.Log("Reloading...");
        gunData.m_bReloading = true;
        yield return new WaitForSeconds(gunData.m_fReloadTime);
        gunData.m_iCurrentAmmo = gunData.m_iClipSize;
        gunData.m_bReloading = false;
        Debug.Log("Reload complete.");
    }

    private void OnGunShot()
    {
        m_shootAudioPlayer.pitch = Random.Range(0.85f, 1.15f);
        m_shootAudioPlayer.Play();
    }
}
