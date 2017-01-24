using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class Cymbals : MonoBehaviour 
{
    [SerializeField] private BeatListener m_beatListener;
    [SerializeField] private Transform m_transform;

    protected void OnEnable()
    {
        // set functions relatie to frequencies you want this object to listen to
        BeatCounter.OnPressenceNotify += m_beatListener.OnPressenceNotify;
    }

    protected void OnDisable()
    {
        BeatCounter.OnPressenceNotify -= m_beatListener.OnPressenceNotify;
    }

//    protected void Start()
//    {
//        m_transform = this.transform;
//    }

    protected void Update()
    {
        Vector3 localPos = m_transform.localPosition;

        if(( m_beatListener.FeRangeMask & FrequencyRange.Pressence ) == FrequencyRange.Pressence )
        {
            localPos.y = -1.0f;
        }

        if( localPos.y < 0 )
        {
            localPos.y += 0.1f;
        }

        if( localPos.y >= 0 )
        {
            localPos.y = 0;
        }

        m_transform.localPosition = localPos;
    }
}
