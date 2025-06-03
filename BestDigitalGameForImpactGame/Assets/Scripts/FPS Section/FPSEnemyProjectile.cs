using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FPSEnemyProjectile : MonoBehaviour
{
    [Header("PresetMats")]
    [SerializeField] private Material m_MatRed;
    [SerializeField] private Material m_MatGreen;
    [SerializeField] private Material m_MatBlue;

    private char m_cColour = 'n';
    private float m_fSpawnTime;
    private float m_fLifeTime = 5.0f;

    private Renderer m_Renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_fSpawnTime = Time.time;
        m_Renderer = GetComponent<Renderer>();
        
        //Randomizing colour
        int i = Random.Range(0, 3);
        switch (i)
        {
            case 0:
                SetColour('r');
                break;
            case 1:
                SetColour('g');
                break;
            case 2:
                SetColour('b');
                break;
        }
    }

    public void SetColour(char _cColour)
    {
        m_cColour = _cColour;
        switch (_cColour)
        {
            case 'r':
                m_Renderer.material = m_MatRed;
                break;
            case 'g':
                m_Renderer.material = m_MatGreen;
                break;
            case 'b':
                m_Renderer.material = m_MatBlue;
                break;
            default:
                //Should not be reached
                Debug.LogError("Invalid _cColour: FPSEnemyProjectile - SetColour()");
                break;
            
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out FPSAIController AIController))
        {
            //Hit AI Obj
            AIController.ProjectileHit(m_cColour);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_fSpawnTime + m_fLifeTime <= Time.time)
        {
            //Despawn
            Destroy(gameObject);
        }
    }
}
