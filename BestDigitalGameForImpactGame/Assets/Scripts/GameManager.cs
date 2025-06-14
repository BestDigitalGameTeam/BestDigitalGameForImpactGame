using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class GameManager : SingletonPersistent<GameManager>
{
    public UnityEvent<string> LoadLevel;
    public UnityEvent VoidLoaded;
    public Transform SpawnPos;
    
    public UnityEvent<bool> PlayerPressedReinforcementButton;
    public UnityEvent ActivateReinforcementButtons;

    public float MasterVolume = 1.0f;
    public float EffectsVolume = 1.0f;
    public float DialogueVolume = 1.0f;


    private void Start()
    {
        LoadLevel.AddListener(LoadScene);
    }

    private void LoadScene(string _name)
    {
        StartCoroutine(LoadSceneAsync(_name));
    }

    private IEnumerator LoadSceneAsync(string _LevelName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_LevelName);

        Debug.Log("Loading Level");
        while (!asyncLoad.isDone) { yield return null; }
        SpawnPos = GameObject.Find("Spawn").transform;

        GameObject player = GameObject.FindWithTag("Player");
        CharacterController controller = player.GetComponent<CharacterController>();
        // Temporarily disable CharacterController to avoid physics glitches
        controller.enabled = false;
        player.transform.position = SpawnPos.position;
        controller.enabled = true;

        if (_LevelName == "Void") VoidLoaded.Invoke();
    }

    public void SetMasterVolume(System.Single _vol)
    {
        MasterVolume = _vol;
    }
    public void SetEffectsVolume(System.Single _vol)
    {
        EffectsVolume = _vol;
    }
    public void SetDialogueVolume(System.Single _vol)
    {
        DialogueVolume = _vol;
    }
}
