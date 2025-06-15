using System;
using UnityEngine;

// Handles player shooting input
public class PlayerShoot : MonoBehaviour
{
    public static Action ShootInput; // Static event to notify when the player attempts to shoot

    // Called once per frame
    private void Update()
    {
        // Check if the left mouse button is held down
        if (Input.GetMouseButton(0)) // 0 = Left Mouse Button
            ShootInput?.Invoke(); // Invoke the ShootInput event if it has subscribers (null-safe)
        // ---
    }
    // ---
}
// ---