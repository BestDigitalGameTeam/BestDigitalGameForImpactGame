using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class scrDoorTrigger : MonoBehaviour
{
    public UnityEvent DoorUsed;
    public GenreBias doorType = GenreBias.None;

    [SerializeField] private string[] m_LevelNames;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DoorUsed.Invoke();
            AnnouncerAlgorithm.Instance.IncreaseGenreBias(doorType, 1);
            GameManager.Instance.LoadLevel.Invoke(m_LevelNames[GameManager.Instance.GetNextDoor(doorType)]);
        }
    }
}
