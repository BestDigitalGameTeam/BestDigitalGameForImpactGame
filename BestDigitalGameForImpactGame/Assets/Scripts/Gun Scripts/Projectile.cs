using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 5.0f;

    private void Start()
    {
        Destroy(gameObject, lifeTime); // Auto-destroy
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Hit: {collision.gameObject.name}");
        Destroy(gameObject); // Destroy on hit
    }
}