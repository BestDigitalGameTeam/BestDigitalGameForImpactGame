using System;
using UnityEngine;

public class DeathPlaneScript : MonoBehaviour
{
    public Transform SpawnPos;

    private void Start()
    {
        SpawnPos = GameManager.Instance.SpawnPos;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!SpawnPos) SpawnPos = GameManager.Instance.SpawnPos;
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                // Temporarily disable CharacterController to avoid physics glitches
                controller.enabled = false;
                other.transform.position = SpawnPos.position;
                controller.enabled = true;
                // ---
            }
            else
            {
                other.transform.position = SpawnPos.position;
            }
        }
    }
}
