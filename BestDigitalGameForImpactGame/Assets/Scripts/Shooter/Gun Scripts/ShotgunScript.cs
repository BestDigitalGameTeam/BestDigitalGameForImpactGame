using UnityEngine;
using System.Collections;

public class ShotgunScript : Gun
{
    [SerializeField] int m_iPelletCount = 8;           // Number of pellets per shot
    [SerializeField] float m_fSpreadAngle = 10.0f;     // Max angle of spread in degrees

    private void Start()
    {
        base.Start();
        gunData.m_iCurrentAmmo = gunData.m_iClipSize;
    }

    private void Update()
    {
        m_fTimeSinceLastShot += Time.deltaTime;
    }

    public override void Shoot()
    {
        if (gunData.m_iCurrentAmmo <= 0 || !CanShoot()) return;

        for (int i = 0; i < m_iPelletCount; ++i)
        {
            Vector3 v3Spread = Quaternion.Euler(
                Random.Range(-m_fSpreadAngle, m_fSpreadAngle),
                Random.Range(-m_fSpreadAngle, m_fSpreadAngle),
                0) * projectileSpawnPoint.forward;

            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.LookRotation(v3Spread));
            Rigidbody rigidBodyProjectile = projectile.GetComponent<Rigidbody>();
            rigidBodyProjectile.linearVelocity = v3Spread * gunData.m_fProjectileSpeed;
        }

        --gunData.m_iCurrentAmmo;
        m_fTimeSinceLastShot = 0;

        OnGunShot();
    }

    private bool CanShoot() =>
        !gunData.m_bReloading && m_fTimeSinceLastShot >= 60.0f / gunData.m_fFireRate;

    public override void StartReload() => StartCoroutine(Reload());
    public override bool IsReloading() => gunData.m_bReloading;
    public override bool IsAmmoFull() => gunData.m_iCurrentAmmo >= gunData.m_iClipSize;

    private IEnumerator Reload()
    {
        gunData.m_bReloading = true;
        yield return new WaitForSeconds(gunData.m_fReloadTime);
        gunData.m_iCurrentAmmo = gunData.m_iClipSize;
        gunData.m_bReloading = false;
    }
}