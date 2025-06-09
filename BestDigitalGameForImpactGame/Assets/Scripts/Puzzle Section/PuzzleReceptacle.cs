using System.Linq;
using UnityEngine;

// Detects when a Puzzle Object is placed into the receptacle
public class PuzzleReceptacle : MonoBehaviour
{
    private enum ReceptacleType { Cube, Sphere, Pyramid }
    [SerializeField] private ReceptacleType eReceptacleType;
    [SerializeField] private Gate gate;

    [SerializeField] private bool bIsOccupied;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object's tag matches the receptacle type
        if (bIsOccupied || !other.CompareTag(GetExpectedTag())) return;

        bIsOccupied = true;
        SnapToCenter(other.transform);
        Debug.Log($"{eReceptacleType} puzzle piece placed.");

        if (AllReceptaclesFilled())
            gate.Open();
        // ---
    }

    private void OnTriggerExit(Collider other)
    {
        if (!bIsOccupied || !other.CompareTag(GetExpectedTag())) return;
        
        bIsOccupied = false;
        Debug.Log($"{eReceptacleType} puzzle piece removed.");

        gate.Close();
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
    
    private bool AllReceptaclesFilled()
    {
        PuzzleReceptacle[] allReceptacles = FindObjectsOfType<PuzzleReceptacle>();
        return allReceptacles.All(receptacle => receptacle.gate != gate || receptacle.bIsOccupied);
    }

/*
    public bool IsOccupied() => bIsOccupied;
*/
}
// ---
