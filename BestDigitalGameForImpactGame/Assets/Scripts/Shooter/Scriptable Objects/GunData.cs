using UnityEngine;

// The base info about the weapon and weapon performance, to be applied to any gun type
[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{
    [Header("Info")] 
    public new string m_sWeaponName;
    
    [Header("Shooting")]
    public float m_fDamage;
    public float m_fRange;

    [Header("Reloading")] 
    public int m_iCurrentAmmo;
    public int m_iClipSize;
    public float m_fFireRate;
    public float m_fReloadTime;
    public float m_fProjectileSpeed;
    [HideInInspector] public bool m_bReloading;
}
// ---