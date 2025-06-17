using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private GameObject m_goWeaponPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponManager weaponManager = other.GetComponentInChildren<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.EquipWeapon(m_goWeaponPrefab);
                Destroy(gameObject); // remove pickup from scene
            }
        }
    }
}