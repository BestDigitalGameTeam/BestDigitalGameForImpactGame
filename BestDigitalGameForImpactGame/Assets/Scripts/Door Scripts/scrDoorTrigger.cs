using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrDoorTrigger : MonoBehaviour
{
    public string doorColor = "Red"; // Set this in Inspector
    public GenreBias doorType = GenreBias.None;

    [SerializeField] private string[] m_LevelNames;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AnnouncerAlgorithm.Instance.IncreaseGenreBias(doorType, 1);
            int levelKey = Random.Range(0, m_LevelNames.Length);
            GameManager.Instance.LoadLevel.Invoke(m_LevelNames[levelKey]);
        }
    }
}
