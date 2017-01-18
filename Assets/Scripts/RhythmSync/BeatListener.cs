using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class BeatListener : MonoBehaviour
{
    [ Range( 0, 500 )]
    [SerializeField] private float m_fBeatWindow = 10f;

    private BeatType m_beatMask = BeatType.None;

    public BeatType BeatMask{ get{ return m_beatMask; }}

    public void Notify( BeatType p_beatType )
    {
        m_beatMask |= p_beatType;
        StartCoroutine( WaitOnBeat( p_beatType ));
    }

    private IEnumerator WaitOnBeat( BeatType p_beatType )
    {
        yield return new WaitForSeconds( m_fBeatWindow * 0.001f );
        m_beatMask ^= p_beatType;
    }
}
