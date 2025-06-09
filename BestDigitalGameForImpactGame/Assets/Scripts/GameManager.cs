using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class GameManager : SingletonPersistent<GameManager>
{
    public UnityEvent<string> LoadLevel;
    public UnityEvent VoidLoaded;

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
        if (_LevelName == "Void") VoidLoaded.Invoke();
    }
}
