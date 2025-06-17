using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using UnityEngine.Events;
using AYellowpaper.SerializedCollections; // for serialised dictionary

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
    [SerializeField] private float m_fAnchoringBiasWeight = 5.0f;
    // how much confirmatin bias is affecting weighting
    [SerializeField] private float m_fConfirmationBiasWeight = 2.0f;

    [SerializeField] private float m_PlayerReinforcementAmount = 1.0f;

    [SerializeField] private GameObject ShooterDoorPrefab;
    private GameObject ShooterDoor;
    [SerializeField] private GameObject PuzzleDoorPrefab;
    private GameObject PuzzleDoor;
    [SerializeField] private GameObject PlatformerDoorPrefab;
    private GameObject PlatformerDoor;

    [SerializeField] private int m_TimesVisitedVoid = 1;

    #endregion

    #region events
    public UnityEvent<int> AnnouncerDialogue;
    public UnityEvent DialogueEnded;

    [SerializedDictionary("Audio Key, Audio Clip")] public SerializedDictionary<int, AudioClip> m_DialogueAudios;
    public AudioSource AnnouncerAudioSource;

    // event-based variables
    private int m_AnchBiasKey;

    private bool m_bReinforcementComplete = false;
    private int m_iTimesDeniedThisVoid = 0;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.VoidLoaded.AddListener(EnterVoid);
        GameManager.Instance.PlayerPressedReinforcementButton.AddListener(PlayerPressedButton);
        GameManager.Instance.PauseGame.AddListener(PauseDialogue);
    }

    private void EnterVoid()
    {
        m_iTimesDeniedThisVoid = 0;
        m_TimesVisitedVoid++;

        ShooterDoor = Instantiate(ShooterDoorPrefab, new Vector3(10.0f, 0.0f, -20.0f), new Quaternion());
        PuzzleDoor = Instantiate(PuzzleDoorPrefab, new Vector3(10.0f, 0.0f, 0.0f), new Quaternion());
        PlatformerDoor = Instantiate(PlatformerDoorPrefab, new Vector3(10.0f, 0.0f, 20.0f), new Quaternion());
        ShooterDoor.SetActive(false);
        PuzzleDoor.SetActive(false);
        PlatformerDoor.SetActive(false);

        CalculateBiasWeightings();
        BeginEventSequence();
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
        if (_biasType == m_CurrentBias && _amt > 0.0f) confAmount = m_fConfirmationBiasWeight;
        else if (_biasType == m_CurrentBias && _amt < 0.0f) confAmount = -1.0f;
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
                    StartCoroutine(InvokeEventAfterTime<string>(GameManager.Instance.LoadLevel, "ShooterLevel1", 15.0f));
                }
                break;
            case GenreBias.Platformer:
                {
                    m_fGenreBias_Platformer += m_fAnchoringBiasWeight;
                    StartCoroutine(InvokeEventAfterTime<string>(GameManager.Instance.LoadLevel, "PlatformerLevel1", 15.0f));
                }
                break;
            case GenreBias.Puzzle:
                { 
                    m_fGenreBias_Puzzle += m_fAnchoringBiasWeight; 
                    StartCoroutine(InvokeEventAfterTime<string>(GameManager.Instance.LoadLevel, "PuzzleLevel1", 15.0f));
                }
                break;
        }
    }

    private void BeginEventSequence()
    {
        if (m_TimesVisitedVoid == 1) FirstEventSequence(); 
        else if (m_TimesVisitedVoid == 2) SecondEventSequence();
        else if (m_TimesVisitedVoid > 2 && m_TimesVisitedVoid <= 3) // for visit times 3, 4, 5
        {
            // create the buttons, don't check
            StartCoroutine(AskPlayerIfLikedLevel());
        }
        else if (m_TimesVisitedVoid > 4)
        {
            if (m_GenreBiasList[0] - m_GenreBiasList[2] <= 5.0f)
            {
                // TODO: dialogue: 
                // How am I supposed to find the pattern when you act like this? Let me make assumptions! Let me tell you what you want!
                // I've had enough of this. Please leave.
            }
            else if (m_GenreBiasList[0] - m_GenreBiasList[1] <= 8.0f && m_GenreBiasList[1] - m_GenreBiasList[2] >= 10.0f)
            {
                // TODO: dialogue: "I don't want to be wrong... why aren't you being more predictable?"
                // But I can see the pattern...
                // Why don't you start again?

            }
            else
            {
                // TODO: dialogue:
                // Just as I expected. I had already made up my mind but this confirms it.
                // This was a huge success. Thank you participant.
                // Please do not try anything differently next time.
            }
        }
    }

    #endregion

    #region Actions and CoRoutines for general event use
    // play single dialogue based on single key (set in dictionary in editor, caption key must match in UI Manager
    // is a coroutine so it can be waited on
    private IEnumerator PlayDialogue(int _key)
    {
        AnnouncerAudioSource.PlayOneShot(m_DialogueAudios[_key], GameManager.Instance.MasterVolume * GameManager.Instance.DialogueVolume);
        AnnouncerDialogue.Invoke(_key);

        yield return StartCoroutine(AudioLengthTimer(m_DialogueAudios[_key].length));
    }

    // play sequence of dialogue with array of keys
    private IEnumerator PlayDialogueSequence(int[] _keys)
    {
        for (int i = 0; i < _keys.Length; i++)
        {
            AnnouncerAudioSource.PlayOneShot(m_DialogueAudios[_keys[i]], GameManager.Instance.MasterVolume * GameManager.Instance.DialogueVolume);
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
            AnnouncerAudioSource.PlayOneShot(m_DialogueAudios[_keys[i]], GameManager.Instance.MasterVolume * GameManager.Instance.DialogueVolume);
            AnnouncerDialogue.Invoke(_keys[i]);

            yield return StartCoroutine(AudioLengthTimer(m_DialogueAudios[_keys[i]].length));
        }
        int key = _getKey();
        AnnouncerAudioSource.PlayOneShot(m_DialogueAudios[key], GameManager.Instance.MasterVolume * GameManager.Instance.DialogueVolume);
        AnnouncerDialogue.Invoke(key);

        yield return StartCoroutine(AudioLengthTimer(m_DialogueAudios[key].length));
    }

    // times length of audio and invokes event when dialogue ends
    // used by UI Manager to remove captions/subtitles off screen
    private IEnumerator AudioLengthTimer(float _time)
    {
        yield return new WaitForSeconds(_time);
        DialogueEnded.Invoke();
    }

    private IEnumerator InvokeEventAfterTime(UnityEvent _event, float _time)
    {
        yield return new WaitForSeconds(_time);
        _event.Invoke();
    }

    // overloaded for event with parameters
    private IEnumerator InvokeEventAfterTime<T>(UnityEvent<T> _event, T _args, float _time)
    {
        yield return new WaitForSeconds(_time);
        _event.Invoke(_args);
    }

    private IEnumerator RunFunctionAfterTime<T>(System.Action<T> _func, T _args, float _time)
    {
        yield return new WaitForSeconds(_time);
        _func.Invoke(_args);
    }
    #endregion

    #region scripted event sequences
    // Initial event sequence
    void FirstEventSequence()
    {
        StartCoroutine(PlayDialogueSequence(new int[3] { 0, 1, 9910}, () => m_AnchBiasKey));
        StartCoroutine(GetFirstKeyForAnchoringBias());
    }

    // event sequence after first level
    void SecondEventSequence()
    {
        StartCoroutine(PlayDialogueSequence(new int[2] { 20, 21 }));
        StartCoroutine(EnableDoorsAfterTime(true, true, true, 5.0f));
    }

    // coroutine for getting players first action
    // does not start straight away, has a short wait (WaitForSeconds)
    private IEnumerator GetFirstKeyForAnchoringBias()
    {
        yield return new WaitForSeconds(5.0f);
        KeyCode[] keysToCheck = { KeyCode.LeftControl, KeyCode.LeftShift, KeyCode.Space, KeyCode.Mouse0, KeyCode.E , KeyCode.R};

        float shooter = 0.0f;
        float puzzle = 0.0f;
        float platformer = 0.0f;
        float timer = 0.0f;

        while (timer < 8.0f)
        {
            timer += 1.0f * Time.deltaTime;
            foreach (KeyCode key in keysToCheck)
            {
                if (Input.GetKeyDown(key))
                {
                    if (key == KeyCode.LeftControl || key == KeyCode.E) puzzle += 1.0f;
                    else if (key == KeyCode.Space || key == KeyCode.LeftShift) platformer += 1.0f;
                    else if (key == KeyCode.Mouse0 || key == KeyCode.R) shooter += 1.0f;
                }
            }
            yield return null;
        }

        List<float> initials = new List<float> { shooter, platformer, puzzle };
        initials.Sort();
        initials.Reverse();
        if (initials[0] == shooter)
        {
            SetInitialBias(GenreBias.Shooter);
            m_AnchBiasKey = 13;
        }
        else if (initials[0] == platformer)
        {
            SetInitialBias(GenreBias.Platformer);
            m_AnchBiasKey = 12;
        }
        else if (initials[0] == puzzle)
        {
            SetInitialBias(GenreBias.Puzzle);
            m_AnchBiasKey = 11;
        }
    }

    private void PauseDialogue(bool _paused)
    {
        if (_paused) AnnouncerAudioSource.Pause();
        else AnnouncerAudioSource.UnPause();
    }

    private void EnableDoors(bool _bShooter, bool _bPuzzle, bool _bPlatformer)
    {
        ShooterDoor.SetActive(_bShooter);
        PuzzleDoor.SetActive(_bPuzzle);
        PlatformerDoor.SetActive(_bPlatformer);
    }

    private IEnumerator EnableDoorsAfterTime(bool _bShooter, bool _bPuzzle, bool _bPlatformer, float _time)
    {
        yield return new WaitForSeconds(_time);
        EnableDoors(_bShooter, _bPuzzle, _bPlatformer);
    }

    #endregion

    #region Bias Event Sequences (Scripted but chosen on weight)

    // General event - tell player they liked the level
    private IEnumerator AskPlayerIfLikedLevel()
    {
        yield return StartCoroutine(PlayDialogue(30)); 

        // TODO: spawn the buttons
        GameManager.Instance.ActivateReinforcementButtons.Invoke();

        yield return new WaitUntil(() => m_bReinforcementComplete); m_bReinforcementComplete = false;
    }

    private void PlayerPressedButton(bool _reinforced)
    {
        if (_reinforced)
        {
            IncreaseGenreBias(m_CurrentBias, m_PlayerReinforcementAmount);
            CalculateBiasWeightings();

            if (m_GenreBiasList[0] - m_GenreBiasList[1] >= 10.0f)
            {
                // Glad you liked it so much! Do it again
                StartCoroutine(PlayDialogue(40));
                StartCoroutine(EnableDoorsAfterTime(m_CurrentBias == GenreBias.Shooter, m_CurrentBias == GenreBias.Puzzle, m_CurrentBias == GenreBias.Platformer, m_DialogueAudios[40].length));
            }
            else
            {
                // I knew I was right!
                // Two options, you want these two
                StartCoroutine(PlayDialogueSequence(new int[3] { 41, 42, 43 }));
                StartCoroutine(EnableDoorsAfterTime(m_GenreBiasList[0] == m_fGenreBias_Shooter || m_GenreBiasList[1] == m_fGenreBias_Shooter,
                            m_GenreBiasList[0] == m_fGenreBias_Puzzle || m_GenreBiasList[1] == m_fGenreBias_Puzzle,
                            m_GenreBiasList[0] == m_fGenreBias_Platformer || m_GenreBiasList[1] == m_fGenreBias_Platformer, 10.0f));
            }
            m_bReinforcementComplete = true;
        }
        else
        {
            IncreaseGenreBias(m_CurrentBias, -m_PlayerReinforcementAmount);
            CalculateBiasWeightings();

            switch (m_iTimesDeniedThisVoid)
            {
                case 0:
                    {
                        StartCoroutine(PlayDialogue(50));
                        
                        if (m_CurrentBias == m_InitialAnchorBias)
                        {
                            // "But you liked this level first!"
                            // "Lets try this again"
                            // Show more of this content?
                            StartCoroutine(PlayDialogueSequence(new int[3] {61, 62, 30 }));
                            StartCoroutine(InvokeEventAfterTime(GameManager.Instance.ActivateReinforcementButtons, 7.0f));
                        }
                        else
                        {
                            // Maybe I was right at first?
                            // More of the content I first showed you?
                            StartCoroutine(PlayDialogueSequence(new int[3] { 991, 65, 66 }));
                            IncreaseGenreBias(m_CurrentBias, -m_fAnchoringBiasWeight);
                            CalculateBiasWeightings();
                            StartCoroutine(InvokeEventAfterTime(GameManager.Instance.ActivateReinforcementButtons, 7.0f));
                        }
                    }
                    break;
                case 1:
                    {
                        // TODO: "Still no?"
                        StartCoroutine(PlayDialogue(70));

                        if (m_GenreBiasList[0] - m_GenreBiasList[1] >= 10.0f)
                        {
                            // TODO: dialogue "But I was correct before!"
                            StartCoroutine(PlayDialogueSequence(new int[3] { 991, 71, 991 }));

                            // randomly either make ONLY top bias or only NOT top bias
                            int onlyOne = Random.Range(0, 2);
                            if (onlyOne == 0)
                            {
                                // TODO: "Fine! Have it your way!"
                                StartCoroutine(PlayDialogue(72));
                                
                                EnableDoors(m_GenreBiasList[2] == m_fGenreBias_Shooter || m_GenreBiasList[1] == m_fGenreBias_Shooter,
                                            m_GenreBiasList[2] == m_fGenreBias_Puzzle || m_GenreBiasList[1] == m_fGenreBias_Puzzle,
                                            m_GenreBiasList[2] == m_fGenreBias_Platformer || m_GenreBiasList[1] == m_fGenreBias_Platformer);
                            }
                            else
                            {
                                // TODO: No! You have to like it! There's no other option!
                                // You just need more of what you liked! More content! 
                                StartCoroutine(PlayDialogueSequence(new int[2] { 73, 74 }));
                                EnableDoors(m_GenreBiasList[0] == m_fGenreBias_Shooter, m_GenreBiasList[0] == m_fGenreBias_Puzzle, m_GenreBiasList[0] == m_fGenreBias_Platformer); // make it spawn lots all the same if time
                            }
                        }
                        else
                        {
                            // TODO: I don't believe you! You should try it again, then you'll see that I'm right!
                            // The pattern is what I believe it is! You can't change my mind.
                            StartCoroutine(PlayDialogueSequence(new int[2] { 75, 76 }));

                            StartCoroutine(InvokeEventAfterTime(GameManager.Instance.ActivateReinforcementButtons, 6.0f));
                        }
                    }
                    break;
                case 2:
                    {
                        // TODO: This is getting out of hand!
                        // If I am wrong, what is the pattern then?
                        StartCoroutine(PlayDialogueSequence(new int[2] { 80, 81 }));

                        if (m_GenreBiasList[0] - m_GenreBiasList[2] <= 10.0f)
                        {
                            // TODO: Fine, have all the options.

                            EnableDoors(true, true, true);
                        }
                        else
                        {
                            // You can have two options. The two I think you might want. 
                            StartCoroutine(PlayDialogueSequence(new int[2] { 42, 43 }));

                            StartCoroutine(EnableDoorsAfterTime(m_GenreBiasList[0] == m_fGenreBias_Shooter || m_GenreBiasList[1] == m_fGenreBias_Shooter,
                            m_GenreBiasList[0] == m_fGenreBias_Puzzle || m_GenreBiasList[1] == m_fGenreBias_Puzzle,
                            m_GenreBiasList[0] == m_fGenreBias_Platformer || m_GenreBiasList[1] == m_fGenreBias_Platformer, 7.0f));
                        }
                    }
                    break;
            }
            m_iTimesDeniedThisVoid++;
        }
        m_bReinforcementComplete = true;

        CalculateBiasWeightings();
    }
    #endregion
}

