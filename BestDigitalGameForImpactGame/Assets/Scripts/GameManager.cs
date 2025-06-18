using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using UnityEditor;

public class GameManager : SingletonPersistent<GameManager>
{
    [SerializeField] private UIManager m_UIManager;
    
    public UnityEvent<string> LoadLevel;
    public UnityEvent VoidLoaded;
    public Transform SpawnPos;
    
    public UnityEvent<bool> PlayerPressedReinforcementButton;
    public UnityEvent ActivateReinforcementButtons;
    public UnityEvent<bool> PauseGame;
    public UnityEvent ResetGame;

    public float MasterVolume = 1.0f;
    public float EffectsVolume = 1.0f;
    public float DialogueVolume = 1.0f;

    public int TimesThroughShooterDoor = 0;
    public int TimesThroughPuzzleDoor = 0;
    public int TimesThroughPlatformerDoor = 0;

    public bool isPaused { get; private set; } = false;


    private void Start()
    {
        LoadLevel.AddListener(LoadScene);
        PauseGame.AddListener(PauseMenu);
        ResetGame.AddListener(GameReset);
    }

    private void LoadScene(string _name)
    {
        StartCoroutine(LoadSceneAsync(_name));
    }

    public void LoadVoid()
    {
        m_UIManager.HideMainMenu();
        LoadScene("Void");
    }

    public void ShowOptions()
    {
        m_UIManager.ShowPauseMenu(true);
    }

    private IEnumerator LoadSceneAsync(string _LevelName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_LevelName);

        Debug.Log("Loading Level");
        while (!asyncLoad.isDone) { yield return null; }
        SpawnPos = GameObject.Find("Spawn").transform;
        PlayerHealth.Instance.ResetHealth();

        GameObject player = GameObject.FindWithTag("Player");
        CharacterController controller = player.GetComponent<CharacterController>();
        // Temporarily disable CharacterController to avoid physics glitches
        controller.enabled = false;
        player.transform.position = SpawnPos.position;
        controller.enabled = true;

        if (_LevelName == "Void") VoidLoaded.Invoke();
    }

    private void OnApplicationPause(bool pause)
    {
        PauseGame.Invoke(pause);
    }

    public void ResumeButtonClick()
    {
        PauseGame.Invoke(false);
    }

    private void PauseMenu(bool _paused)
    {
        if (_paused)
        {
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1.0f;

            if (FindFirstObjectByType<ResetScript>() == null)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
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
    public void Quit()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
    public void GameReset()
    {
        TimesThroughShooterDoor = 0;
        TimesThroughPuzzleDoor = 0;
        TimesThroughPlatformerDoor = 0;
    }

    public void PlatDoor()
    {
        TimesThroughPlatformerDoor++;
    }
    public void ShootDoor()
    {
        TimesThroughShooterDoor++;
    }
    public void PuzzDoor()
    {
        TimesThroughPuzzleDoor++;
    }

    public int GetNextDoor(GenreBias _bias)
    {
        switch (_bias)
        {
            case GenreBias.None:
                break;
            case GenreBias.Shooter:
                return TimesThroughShooterDoor;
            case GenreBias.Platformer:
                return TimesThroughPlatformerDoor;
            case GenreBias.Puzzle:
                return TimesThroughPuzzleDoor;
            default:
                break;
        }
        return 0;
    }
}
