using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : SingletonBehaviour<SoundSystem>
{
    [SerializeField] private AudioSource m_sourceBGM;

    public delegate void SoundSystemAction( double p_syncTime );
    public static event SoundSystemAction OnBGMPlay;
    public static event SoundSystemAction OnBGMStop;

    public AudioSource SourceBGM{ get{ return m_sourceBGM; }}

    public void OnClickPlay()
    {
        double initTime = AudioSettings.dspTime;
        m_sourceBGM.PlayScheduled( initTime );

        if( OnBGMPlay != null )
        {
            OnBGMPlay( initTime );
        }
    }

    public void OnClickStop()
    {
        m_sourceBGM.Stop();

        if( OnBGMStop != null )
        {
            OnBGMStop( 0 );
        }
    }
}
