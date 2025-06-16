using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Transform m_tWeaponHolder;
    private GameObject m_goCurrentWeapon;

    public Gun CurrentGunScript { get; private set; }

    public void EquipWeapon(GameObject _goWeaponPrefab)
    {
        if (m_goCurrentWeapon != null)
            Destroy(m_goCurrentWeapon);

        m_goCurrentWeapon = Instantiate(_goWeaponPrefab, m_tWeaponHolder);
        m_goCurrentWeapon.transform.localPosition = Vector3.zero;
        m_goCurrentWeapon.transform.localRotation = Quaternion.identity;

        CurrentGunScript = m_goCurrentWeapon.GetComponent<Gun>();
    }
}
