using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using UnityEngine.Events;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting; // for serialised dictionary

// File Authour: Charli 
// all the switch statements are kinda gross anyone have better idea?

public enum GenreBias
{
    None,
    Shooter,
    Platformer,
    Puzzle,
}

// Class for changing weightings and weighing up biases
// Main AI brain for calling events and managing AI algorithm
public class AnnouncerAlgorithm : SingletonPersistent<AnnouncerAlgorithm>
{
    #region variables!!
    [SerializeField] private float m_fGenreBias_Shooter;
    [SerializeField] private float m_fGenreBias_Platformer;
    [SerializeField] private float m_fGenreBias_Puzzle;

    [SerializeField] private List<float> m_GenreBiasList;

    // which bias the anouncer is initially anchored to
    // AI will weight this too high and keep reference of which type to make sure it can be referred back to
    // to highlight anchoring bias
    [SerializeField] private GenreBias m_InitialAnchorBias = GenreBias.None;
    public GenreBias m_CurrentBias = GenreBias.None;

    // how much the anchoring bias is affecting the AI's decision making
    [SerializeField] private float m_fAnchoringBiasWeight = 5;
    // how much confirmatin bias is affecting weighting
    [SerializeField] private float m_fConfirmationBiasWeight = 2;

    [SerializeField] private float m_PlayerReinforcementAmount = 1;

    [SerializeField] private GameObject ShooterDoor;
    [SerializeField] private GameObject PuzzleDoor;
    [SerializeField] private GameObject PlatformerDoor;

    [SerializeField] private int m_TimesVisitedVoid = 1;

    #endregion

    #region events
    public UnityEvent<int> AnnouncerDialogue;
    public UnityEvent DialogueEnded;

    [SerializedDictionary("Audio Key, Audio Clip")] public SerializedDictionary<int, AudioClip> m_DialogueAudios;
    public AudioSource AnnouncerAudioSource;

    // event-based variables
    private int m_AnchBiasKey;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.VoidLoaded.AddListener(CalculateBiasWeightings);
        ShooterDoor.SetActive(false);
        PuzzleDoor.SetActive(false);
        PlatformerDoor.SetActive(false);
        FirstEventSequence();
    }

    #region Algorithm weights and calculations
    public void CalculateBiasWeightings()
    {
        // highest weighted bias
        // but prioritise current bias, don't want to be changing all the time

        // add weighting to current bias, AI biased towards what it already believes

        m_GenreBiasList = new()
        {
            m_fGenreBias_Shooter,
            m_fGenreBias_Platformer,
            m_fGenreBias_Puzzle
        };

        m_GenreBiasList.Sort(); // might need to change, may not sort in correct order
        m_GenreBiasList.Reverse();

        if (m_GenreBiasList[0] == m_fGenreBias_Shooter) m_CurrentBias = GenreBias.Shooter;
        else if (m_GenreBiasList[0] == m_fGenreBias_Platformer) m_CurrentBias = GenreBias.Platformer;
        else if (m_GenreBiasList[0] == m_fGenreBias_Puzzle) m_CurrentBias = GenreBias.Puzzle;

    }

    // accessed from player/doors etc to increase/decrease AI bias
    public void IncreaseGenreBias(GenreBias _biasType, float _amt)
    {
        float confAmount = 0.0f;
        if (_biasType == m_CurrentBias) confAmount = m_fConfirmationBiasWeight;
        switch (_biasType)
        {
            case GenreBias.Shooter:
                { m_fGenreBias_Shooter += _amt + confAmount; }
                break;
            case GenreBias.Platformer:
                { m_fGenreBias_Platformer += _amt + confAmount; }
                break;
            case GenreBias.Puzzle:
                { m_fGenreBias_Puzzle += _amt + confAmount; }
                break;
            default:
                break;
        }
    }

    // set initial bias then invoke the spawn level event after time
    public void SetInitialBias(GenreBias _initBias)
    {
        m_InitialAnchorBias = _initBias;
        switch (m_InitialAnchorBias)
        {
            case GenreBias.Shooter:
                { 
                    m_fGenreBias_Shooter += m_fAnchoringBiasWeight;
                    StartCoroutine(InvokeEventAfterTime<string>(GameManager.Instance.LoadLevel, "ShooterLevel_0", 15.0f));
                }
                break;
            case GenreBias.Platformer:
                {
                    m_fGenreBias_Platformer += m_fAnchoringBiasWeight;
                    StartCoroutine(InvokeEventAfterTime<string>(GameManager.Instance.LoadLevel, "PlatformerLevel_0", 15.0f));
                }
                break;
            case GenreBias.Puzzle:
                { 
                    m_fGenreBias_Puzzle += m_fAnchoringBiasWeight; 
                    StartCoroutine(InvokeEventAfterTime<string>(GameManager.Instance.LoadLevel, "PuzzleLevel_0", 15.0f));
                }
                break;
        }
    }

    private void BeginEventSequence()
    {
        if (m_TimesVisitedVoid == 2) SecondEventSequence();
    }

    #endregion

    #region Actions and CoRoutines for general event use
    // play single dialogue based on single key (set in dictionary in editor, caption key must match in UI Manager
    private void PlayDialogue(int _key)
    {
        AnnouncerAudioSource.PlayOneShot(m_DialogueAudios[_key]);
        AnnouncerDialogue.Invoke(_key);

        StartCoroutine(AudioLengthTimer(m_DialogueAudios[_key].length));
    }

    // play sequence of dialogue with array of keys
    private IEnumerator PlayDialogueSequence(int[] _keys)
    {
        for (int i = 0; i < _keys.Length; i++)
        {
            AnnouncerAudioSource.PlayOneShot(m_DialogueAudios[_keys[i]]);
            AnnouncerDialogue.Invoke(_keys[i]);

            yield return StartCoroutine(AudioLengthTimer(m_DialogueAudios[_keys[i]].length));
        }
    }

    // play sequence of dialogue with array of keys, AND a variable key that can be changed elsewhere before its played
    // eg for the anchoring bias, key is changed based on which bias is chosen
    // plays the audio for each bias
    private IEnumerator PlayDialogueSequence(int[] _keys, System.Func<int> _getKey)
    {
        for (int i = 0; i < _keys.Length; i++)
        {
            AnnouncerAudioSource.PlayOneShot(m_DialogueAudios[_keys[i]]);
            AnnouncerDialogue.Invoke(_keys[i]);

            yield return StartCoroutine(AudioLengthTimer(m_DialogueAudios[_keys[i]].length));
        }
        int key = _getKey();
        AnnouncerAudioSource.PlayOneShot(m_DialogueAudios[key]);
        AnnouncerDialogue.Invoke(key);

        yield return StartCoroutine(AudioLengthTimer(m_DialogueAudios[key].length));
    }

    // times length of audio and invokes event when dialogue ends
    // used by UI Manager to remove captions/subtitles off screen
    private IEnumerator AudioLengthTimer(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        DialogueEnded.Invoke();
    }

    private IEnumerator InvokeEventAfterTime(UnityEvent _event, float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        _event.Invoke();
    }

    // overloaded for event with parameters
    private IEnumerator InvokeEventAfterTime<T>(UnityEvent<T> _event, T _args, float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        _event.Invoke(_args);
    }

    private IEnumerator RunFunctionAfterTime<T>(System.Action<T> _func, T _args, float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        _func.Invoke(_args);
    }
    #endregion

    #region scripted event sequences
    // Initial event sequence
    void FirstEventSequence()
    {
        StartCoroutine(PlayDialogueSequence(new int[3] { 0, 1, 995}, () => m_AnchBiasKey));
        StartCoroutine(GetFirstKeyForAnchoringBias());
    }

    // event sequence after first level
    void SecondEventSequence()
    {
        // play dialogue "I hope you enjoyed your first level"
        PlayDialogueSequence(new int[1] { 2 }); // add other dialogue in front when its made
        EnableDoorsAfterTime(true, true, true, 10.0f);
    }

    // coroutine for getting players first action
    // does not start straight away, has a short wait (WaitForSecondsRealtime)
    private IEnumerator GetFirstKeyForAnchoringBias()
    {
        bool biasSet = false;
        yield return new WaitForSecondsRealtime(6);
        KeyCode[] keysToCheck = { KeyCode.LeftControl, KeyCode.LeftShift, KeyCode.Space, KeyCode.Mouse0, KeyCode.E };
        while (!biasSet)
        {
            foreach (KeyCode key in keysToCheck)
            {
                if (Input.GetKeyDown(key))
                {
                    if (key == KeyCode.LeftControl || key == KeyCode.E)
                    {
                        SetInitialBias(GenreBias.Puzzle);
                        m_AnchBiasKey = 4;
                    }
                    else if (key == KeyCode.Space)
                    {
                        SetInitialBias(GenreBias.Platformer);
                        m_AnchBiasKey = 3;
                    }
                    else if (key == KeyCode.Mouse0 || key == KeyCode.LeftShift)
                    {
                        SetInitialBias(GenreBias.Shooter);
                        m_AnchBiasKey = 5;
                    }
                    biasSet = true;
                }
            }
            
            yield return null;
        }
    }

    private void EnableDoors(bool _bShooter, bool _bPuzzle, bool _bPlatformer)
    {
        ShooterDoor.SetActive(_bShooter);
        PuzzleDoor.SetActive(_bPuzzle);
        PlatformerDoor.SetActive(_bPlatformer);
    }

    private IEnumerator EnableDoorsAfterTime(bool _bShooter, bool _bPuzzle, bool _bPlatformer, float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        EnableDoors(_bShooter, _bPuzzle, _bPlatformer);
    }

    #endregion
}

