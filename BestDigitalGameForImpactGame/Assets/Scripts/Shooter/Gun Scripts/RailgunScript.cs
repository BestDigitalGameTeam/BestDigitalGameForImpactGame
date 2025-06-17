using UnityEngine;
using System.Collections;

public class RailgunScript : Gun
{
    private void Start()
    {
        base.Start();
        gunData.m_iCurrentAmmo = gunData.m_iClipSize;
        WeaponHUDKey = 1;
    }

    private void Update()
    {
        m_fTimeSinceLastShot += Time.deltaTime;
    }

    public override GunData GetGunData()
    {
        return gunData;
    }

    public override void Shoot()
    {
        if (gunData.m_iCurrentAmmo <= 0 || !CanShoot()) return;

        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        Rigidbody projectileRigidBody = projectile.GetComponent<Rigidbody>();
        projectileRigidBody.linearVelocity = projectileSpawnPoint.forward * gunData.m_fProjectileSpeed;

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
        UpdateAmmoCount();
    }
}