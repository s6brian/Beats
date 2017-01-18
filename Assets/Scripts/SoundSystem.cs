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

//    protected void Start()
//    {
//        Debug.Log( "channel count: " + m_sourceBGM.clip.channels );
//        Debug.Log( "samples: " + m_sourceBGM.clip.samples );
//    }

//    void Update( )
//    {
//        float[] spectrum = new float[256];
//
//        AudioListener.GetSpectrumData( spectrum, 0, FFTWindow.Rectangular );
//        Debug.Log( "frequency: " + m_sourceBGM.clip.frequency );
//
//        for( int i = 1; i < spectrum.Length-1; i++ )
//        {
//            Debug.DrawLine( new Vector3( i - 1, spectrum[i] + 10, 0 ), new Vector3( i, spectrum[i + 1] + 10, 0 ), Color.red );
//            Debug.DrawLine( new Vector3( i - 1, Mathf.Log( spectrum[i - 1] ) + 10, 2 ), new Vector3( i, Mathf.Log( spectrum[i] ) + 10, 2 ), Color.cyan );
//            Debug.DrawLine( new Vector3( Mathf.Log( i - 1 ), spectrum[i - 1] - 10, 1 ), new Vector3( Mathf.Log( i ), spectrum[i] - 10, 1 ), Color.green );
//            Debug.DrawLine( new Vector3( Mathf.Log( i - 1 ), Mathf.Log( spectrum[i - 1] ), 3 ), new Vector3( Mathf.Log( i ), Mathf.Log( spectrum[i] ), 3 ), Color.blue );
//        }
//    }

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
