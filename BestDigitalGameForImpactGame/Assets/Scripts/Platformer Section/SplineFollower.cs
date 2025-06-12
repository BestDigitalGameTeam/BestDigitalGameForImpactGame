using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{

    public enum SplineLoopType
    {
        //How the platform should loop on the spline
        Once,
        Continuous,
        PingPong,
    }

    public enum Easing
    {
        //Platform Easing
        None,
        EaseIn,
        EaseOut,
        EaseInOut,
    }
    
    public SplineContainer FollowedSpline;
    public Easing SplineEasing;
    public SplineLoopType LoopType;
    public float m_fSpeed = 0.2f;
    private float m_fTime = 0.0f;
    private float m_fLoopMultiplier = 0.0f;

    private Vector3 lastPos;
    private Vector3 deltaPos;

    public Vector3 GetDeltaPos()
    {
        //Getter
        return deltaPos;
    }

    public void StartMovement()
    {
        //Start Movement on platform (only works on SplineLoopType.Once platforms)
        if (LoopType != SplineLoopType.Once)
        {
            //if called on platform that isn't a one time platform
            Debug.LogError("Start Movement called on non-SplineLoopType.Once Moving Platform");
        }

        m_fLoopMultiplier = 1.0f;
    }
    
    void Start()
    {
        lastPos = transform.position;
    }

    private float EaseInCalc(float _fX)
    {
        return (float)(1 - Math.Cos((_fX * Math.PI) / 2));
    }

    private float EaseOutCalc(float _fX)
    {
        return (float)Math.Sin((_fX * Math.PI) / 2);
    }
    
    private float EaseInOutCalc(float _fX)
    {
        return (float)-(Math.Cos(Math.PI * _fX) - 1) / 2;
    }

    void Update()
    {
        //Updating Platform Position based on loop type and easing
        m_fTime += m_fSpeed * m_fLoopMultiplier * Time.deltaTime;
        
        switch (SplineEasing)
        {
            case Easing.None:
                transform.position = FollowedSpline.Spline.EvaluatePosition(m_fTime) + (float3)transform.parent.transform.position;
                break;
            case Easing.EaseIn:
                transform.position = FollowedSpline.Spline.EvaluatePosition(EaseInCalc(m_fTime)) + (float3)transform.parent.transform.position;
                break;
            case Easing.EaseOut:
                transform.position = FollowedSpline.Spline.EvaluatePosition(EaseOutCalc(m_fTime)) + (float3)transform.parent.transform.position;
                break;
            case Easing.EaseInOut:
                transform.position = FollowedSpline.Spline.EvaluatePosition(EaseInOutCalc(m_fTime)) + (float3)transform.parent.transform.position;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (LoopType)
        {
            case SplineLoopType.Once:
                if (m_fTime > 1.0f)
                {
                    m_fLoopMultiplier = 0.0f;
                }
                else
                {
                    m_fLoopMultiplier = 1.0f;
                }
                break;
            case SplineLoopType.Continuous:
                m_fLoopMultiplier = 1.0f;
                if (m_fTime > 1.0f)
                {
                    m_fTime = 0.0f;
                }
                break;
            case SplineLoopType.PingPong:
                
                if (m_fTime >= 1.0f)
                {
                    m_fLoopMultiplier = -1.0f;
                }
                else if (m_fTime <= 0.0f)
                {
                    m_fLoopMultiplier = 1.0f;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        
        deltaPos = transform.position - lastPos;
        lastPos = transform.position;
    }
}

