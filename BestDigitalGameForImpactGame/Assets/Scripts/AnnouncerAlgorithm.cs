using UnityEngine;
using System.Collections.Generic;

// File Authour: Charli 
// all the switch statements are kinda gross anyone have better idea?

public enum GenreBias
{
    Shooter,
    Platformer,
    Puzzle,
}

// Class for changing weightings and weighing up biases
// Main AI brain for calling events and managing AI algorithm
public class AnnouncerAlgorithm : MonoBehaviour
{
    [SerializeField] private float m_fGenreBias_Shooter;
    [SerializeField] private float m_fGenreBias_Platformer;
    [SerializeField] private float m_fGenreBias_Puzzle;

    [SerializeField] private List<float> m_GenreBiasList;

    // which bias the anouncer is initially anchored to
    // AI will weight this too high and keep reference of which type to make sure it can be referred back to
    // to highlight anchoring bias
    [SerializeField] private GenreBias m_InitialAnchorBias;
    public GenreBias m_CurrentBias;

    // how much the anchoring bias is affecting the AI's decision making
    [SerializeField] private float m_fAnchoringBiasWeight = 5;
    // how much confirmatin bias is affecting weighting
    [SerializeField] private float m_fConfirmationBiasWeight = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_GenreBiasList = new();
        m_GenreBiasList.Add(m_fGenreBias_Shooter);
        m_GenreBiasList.Add(m_fGenreBias_Platformer);
        m_GenreBiasList.Add(m_fGenreBias_Puzzle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CalculateBiasWeightings()
    {
        // highest weighted bias
        // but prioritise current bias, don't want to be changing all the time

        // add weighting to current bias, AI biased towards what it already believes
        switch (m_CurrentBias)
        {
            case GenreBias.Shooter:
                { m_fGenreBias_Shooter += m_fConfirmationBiasWeight; }
                break;
            case GenreBias.Platformer:
                { m_fGenreBias_Platformer += m_fConfirmationBiasWeight; }
                break;
            case GenreBias.Puzzle:
                { m_fGenreBias_Puzzle += m_fConfirmationBiasWeight; }
                break;
        }

        m_GenreBiasList.Sort(); // might need to change, ma not sort in correct order
        m_GenreBiasList.Reverse();
    }

    // accessed from player/doors etc to increase/decrease AI bias
    public void IncreaseGenreBias(GenreBias _biasType, float _amt)
    {
        switch (_biasType)
        {
            case GenreBias.Shooter:
                { m_fGenreBias_Shooter += _amt; }
                break;
            case GenreBias.Platformer:
                { m_fGenreBias_Platformer += _amt; }
                break;
            case GenreBias.Puzzle:
                { m_fGenreBias_Puzzle += _amt; }
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
}
