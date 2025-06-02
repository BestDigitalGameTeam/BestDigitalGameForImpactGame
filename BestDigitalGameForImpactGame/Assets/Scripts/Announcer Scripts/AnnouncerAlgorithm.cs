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
public class AnnouncerAlgorithm : Singleton<AnnouncerAlgorithm>
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

    #endregion

    #region events
    public UnityEvent<int> AnnouncerDialogue;
    public UnityEvent DialogueEnded;

    [SerializedDictionary("Audio Key, Audio Clip")] public SerializedDictionary<int, AudioClip> m_DialogueAudios;
    public AudioSource AnnouncerAudioSource;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayDialogue(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void SetInitialBias(GenreBias _initBias)
    {
        m_InitialAnchorBias = _initBias;
        switch (m_InitialAnchorBias)
        {
            case GenreBias.Shooter:
                { m_fGenreBias_Shooter += m_fAnchoringBiasWeight; }
                break;
            case GenreBias.Platformer:
                { m_fGenreBias_Platformer += m_fAnchoringBiasWeight; }
                break;
            case GenreBias.Puzzle:
                { m_fGenreBias_Puzzle += m_fAnchoringBiasWeight; }
                break;
        }
    }

    private void PlayDialogue(int _key)
    {
        AnnouncerAudioSource.PlayOneShot(m_DialogueAudios[_key]);
        AnnouncerDialogue.Invoke(_key);

        StartCoroutine(AudioLengthTimer(m_DialogueAudios[_key].length));
    }

    private IEnumerator AudioLengthTimer(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        DialogueEnded.Invoke();
    }
}

