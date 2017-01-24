using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class Bass : MonoBehaviour
{
    [SerializeField] private BeatListener m_beatListener;

    private Transform m_transform;

    protected void OnEnable()
    {
        // set functions relatie to frequencies you want this object to listen to
        BeatCounter.OnBassNotify += m_beatListener.OnBassNotify;
    }

    protected void OnDisable()
    {
        BeatCounter.OnBassNotify -= m_beatListener.OnBassNotify;
    }

    protected void Start()
    {
        m_transform = this.transform;
    }

    protected void Update()
    {
        Vector3 scale = m_transform.localScale;

        if(( m_beatListener.FeRangeMask & FrequencyRange.Bass ) == FrequencyRange.Bass )
        {
            scale.y = 4f;
        }

        if( scale.y > 1 )
        {
            scale.y -= 0.1f;
        }

        if( scale.y < 1 )
        {
            scale.y = 1;
        }

        m_transform.localScale = scale;
    }
}
