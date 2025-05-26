using System;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public static Action ShootInput;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ShootInput?.Invoke(); // Will avoid a null reference
        }
    }
}
