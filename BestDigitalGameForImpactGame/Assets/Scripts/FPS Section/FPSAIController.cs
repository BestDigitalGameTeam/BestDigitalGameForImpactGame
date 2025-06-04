using System;
using UnityEngine;

public class FPSAIController : MonoBehaviour
{
    private Renderer m_AIRend;
    private int m_iRed;
    private int m_iGreen;
    private int m_iBlue;
    private int m_iTotalHits;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_AIRend = GetComponent<Renderer>();
    }

    public void ProjectileHit(char _cColour)
    {
        //On Enemy projectile hit
        m_iTotalHits++;
        switch (_cColour)
        {
            case 'r':
                m_iRed++;
                break;
            
            case 'g':
                m_iGreen++;
                break;
            case 'b':
                m_iBlue++;
                break;
            default:
                Debug.LogError("Invalid _cColour: FPSAIController - ProjectileHit()");
                //Shouldn't be reached
                break;
        }

        m_AIRend.material.color = new Color((float)m_iRed / m_iTotalHits * 255, (float)m_iGreen / m_iTotalHits * 255,
            (float)m_iBlue / m_iTotalHits * 255);
        
        //Setting new ai colour

    }
}
