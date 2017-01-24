/*
 * source: 
 *      - https://christianfloisand.wordpress.com/2014/01/23/beat-synchronization-in-unity/
 *      - https://github.com/cfloisand/beat-synchronizer-unity
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class BeatListener : MonoBehaviour
{
    [ Range( 0, 500 )]
    [SerializeField] private float m_fBeatWindow = 10f; // in milliseconds

    private BeatType m_beatMask = BeatType.None;
    public BeatType BeatMask{ get{ return m_beatMask; }}

    private FrequencyRange m_feRangeMask = FrequencyRange.None;
    public  FrequencyRange FeRangeMask{ get{ return m_feRangeMask; }}

    /*
        OnSubBassNotify;
        OnBassNotify;
        OnLowMidNotify;
        OnMidRangeNotify;
        OnHighMidNotify;
        OnPressenceNotify;
        OnBrillianceNotify;
    */

    public void OnBassNotify()
    {
        m_feRangeMask |= FrequencyRange.Bass;
        StartCoroutine( WaitOnBeat( FrequencyRange.Bass ));
    }

    public void OnMidRangeNotify()
    {
        m_feRangeMask |= FrequencyRange.Mid;
        StartCoroutine( WaitOnBeat( FrequencyRange.Mid ));
    }

    public void OnPressenceNotify()
    {
        m_feRangeMask |= FrequencyRange.Pressence;
        StartCoroutine( WaitOnBeat( FrequencyRange.Pressence ));
    }

    private IEnumerator WaitOnBeat( FrequencyRange p_feRange )
    {
        yield return new WaitForSeconds( m_fBeatWindow * 0.001f );
        m_feRangeMask ^= p_feRange;
    }
}
