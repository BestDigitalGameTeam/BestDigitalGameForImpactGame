using UnityEngine;

public class scrTeleportTrigger : MonoBehaviour
{
    [Tooltip("The destination to teleport the player to.")]
    public Transform teleportDestination;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player") && teleportDestination != null)
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                // Temporarily disable CharacterController to avoid physics glitches
                controller.enabled = false;
                other.transform.position = teleportDestination.position;
                controller.enabled = true;
                // ---
            }
            else
            {
                other.transform.position = teleportDestination.position;
            }
        }
        // ---
    }
}
