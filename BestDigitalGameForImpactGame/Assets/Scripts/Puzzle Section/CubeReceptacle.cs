using UnityEngine;

// Detects when a Puzzle Object is placed into the receptacle
public class PuzzleReceptacle : MonoBehaviour
{
    private enum ReceptacleType { Cube, Sphere, Pyramid }
    [SerializeField] private ReceptacleType eReceptacleType;

    [SerializeField] private bool bIsOccupied = false;

    private void OnTriggerEnter(Collider other)
    {
        if (bIsOccupied)
            return;

        // Check if the object's tag matches the receptacle type
        if (other.CompareTag(GetExpectedTag()))
        {
            bIsOccupied = true;
            Debug.Log($"{eReceptacleType} puzzle piece placed.");

            SnapToCenter(other.transform); // Optional: Snap the object into place
        }
        // ---
    }

    private void OnTriggerExit(Collider other)
    {
        if (bIsOccupied && other.CompareTag(GetExpectedTag()))
        {
            bIsOccupied = false;
            Debug.Log($"{eReceptacleType} puzzle piece removed.");
        }
    }

    // Optionally snap the cube's position to the center of the receptacle
    private void SnapToCenter(Transform obj)
    {
        obj.position = transform.position;
        obj.rotation = transform.rotation;
    }
    // ---

    private string GetExpectedTag()
    {
        switch (eReceptacleType)
        {
            case ReceptacleType.Cube: return "PuzzleCube";
            case ReceptacleType.Sphere: return "PuzzleSphere";
            case ReceptacleType.Pyramid: return "PuzzlePyramid";
            default: return "";
        }
    }

    public bool IsOccupied() => bIsOccupied;
}
// ---
