using System;
using UnityEngine;

public class DeathPlaneScript : MonoBehaviour
{
    public Transform SpawnPos;

    private void Start()
    {
        if (!SpawnPos)
        {
            //If not preset use gameManager spawn
            SpawnPos = GameManager.Instance.SpawnPos;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!SpawnPos) SpawnPos = GameManager.Instance.SpawnPos;
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller)
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

            foreach (FallAwayScript tempPlatForm in FindObjectsByType<FallAwayScript>(FindObjectsSortMode.None))
            {
                tempPlatForm.Enable();
            }
        }
    }
}
