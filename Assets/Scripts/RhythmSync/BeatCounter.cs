using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class BeatCounter : MonoBehaviour
{
    [SerializeField] private BeatValue m_beatValue = BeatValue.QuarterBeat;
    [SerializeField] private BeatType m_beatType = BeatType.OnBeat;

    [ Range( 0f, 500f )]
    [SerializeField] private float m_fLoopTime = 30.0f;

    [Space]
    [SerializeField] private BeatListener[] m_arrayBeatListeners;

    private AudioSource m_sourceBGM;

    private float m_fNextBeatSample;
    private float m_fSamplePeriod;
    private float m_fCurrentSample;

    private float[]  m_spectrum = new float[1024];
    private float[]  m_subBand  = new float[32];
    // each sub band has 43 energy history
    private float[,] m_eHistory = new float[32, 43];

    public delegate void BeatCounterDelegate( BeatType p_beatType );
    public static event BeatCounterDelegate OnBeatNotify;

    protected void OnEnable()
    {
        SoundSystem.OnBGMPlay += OnBGMPlay;

    }

    protected void OnDisable()
    {
        SoundSystem.OnBGMPlay -= OnBGMPlay;
    }

    protected void Start()
    {
        m_sourceBGM = SoundSystem.Instance.SourceBGM;
        m_fNextBeatSample   = 0.0f;
        m_fSamplePeriod     = 0.0f;
        m_fCurrentSample    = 0.0f;

        //TODO:
        // audioBpm should be automatically computed.
        // 
        // AudioSource.GetSpectrumData might help
        // OR
        // float[] samples = new float[aud.clip.samples * aud.clip.channels];
        //
        // http://archive.gamedev.net/archive/reference/programming/features/beatdetection/

        /*
        float audioBpm = audioSource.GetComponent<BeatSynchronizer>().bpm;
        samplePeriod = (60f / (audioBpm * BeatDecimalValues.values[(int)beatValue])) * audioSource.clip.frequency;

        if (beatOffset != BeatValue.None) {
            sampleOffset = (60f / (audioBpm * BeatDecimalValues.values[(int)beatOffset])) * audioSource.clip.frequency;
            if (negativeBeatOffset) {
                sampleOffset = samplePeriod - sampleOffset;
            }
        }

        samplePeriod *= beatScalar;
        sampleOffset *= beatScalar;
        nextBeatSample = 0f;
        */

        //m_fSamplePeriod = ( 60f / ( 120f * BeatDecimalValues.values[( int )m_beatValue ])) * m_sourceBGM.clip.frequency;
    }

    private void OnBGMPlay( double p_syncTime )
    {
        AudioClip currentClip = m_sourceBGM.clip;

        m_fNextBeatSample = ( float )p_syncTime * currentClip.frequency;
        m_fSamplePeriod   = ( 60f / ( 120f * BeatDecimalValues.values[( int )m_beatValue ])) * m_sourceBGM.clip.frequency;

//        StartCoroutine( "SetBeatPerMinute" );
        StartCoroutine( "BeatCheck" );
    }

//    private void OnBGMStop( double p_syncTime )
//    {
//        StopCoroutine( "SetBeatPerMinute" );
//        StopCoroutine( "BeatCheck" );
//    }

    // beat per minute approximation
    private IEnumerator SetBeatPerMinute()
    {
        while( m_sourceBGM.isPlaying )
        {
            float beatCount = 0f;

            // divide a second into 43 sections and loop through it
            for( int ssIdx = 0; ssIdx < 43; ++ssIdx )
            {
                AudioListener.GetSpectrumData( m_spectrum, 0, FFTWindow.Rectangular );

                for( int subBandIdx = 0; subBandIdx < 32; ++subBandIdx )
                {
                    m_subBand[ subBandIdx ] = 0;

                    for( int spectrumIdx = ( 32 * subBandIdx ); spectrumIdx < ( 32 * ( subBandIdx + 1 )); ++spectrumIdx )
                    {
                        m_subBand[ subBandIdx ] += m_spectrum[ spectrumIdx ] * m_spectrum[ spectrumIdx ]; // square (?)
                        m_subBand[ subBandIdx ] /= 32;
                    }
                }

                // save energy history
                for( int historyIdx = 0; historyIdx < 32; ++historyIdx )
                {
                    // and insert the new sound energy
                    m_eHistory[ historyIdx, 0 ] = m_subBand[ historyIdx ];

                    // shift sound energy buffer 1 index to the right
                    // to push out the oldest cached sound energy
                    for( int historyJdx = 42; historyJdx > 0; --historyJdx )
                    {
                        m_eHistory[ historyIdx, historyJdx ] = m_eHistory[ historyIdx, historyJdx - 1 ];
                    }

                    for( int historyJdx = 1; historyJdx < 43; ++historyJdx )
                    {
                        m_eHistory[ historyIdx, 0 ] += m_eHistory[ historyIdx, historyJdx ];
                        m_eHistory[ historyIdx, 0 ] /= 43;
                    }
                }

                yield return new WaitForSeconds( 0.0233f ); // 1/43
            }

            for( int subBandIdx = 0; subBandIdx < 32; ++subBandIdx )
            {
                beatCount += ( m_subBand[ subBandIdx ] > 250 * m_eHistory[ subBandIdx, 0 ] ) ? 1 : 0;
            }

            beatCount *= 60;
            Debug.Log( "beats per second: " + beatCount );
//            Debug.Log( "beats per minute: " + ( beatCount * 60 ));

//            m_fSamplePeriod = ( 60f / ( 120f * BeatDecimalValues.values[( int )m_beatValue ])) * m_sourceBGM.clip.frequency;
            m_fSamplePeriod = ( 60f / ( beatCount * BeatDecimalValues.values[( int )m_beatValue ])) * m_sourceBGM.clip.frequency;
        }
    }

    private IEnumerator BeatCheck()
    {
        while( m_sourceBGM.isPlaying )
        {
            m_fCurrentSample = ( float )AudioSettings.dspTime * m_sourceBGM.clip.frequency;

            if( m_fCurrentSample >= m_fNextBeatSample )
            {
                foreach( BeatListener beatListener in m_arrayBeatListeners )
                {
                    beatListener.Notify( m_beatType );
                }
                m_fNextBeatSample += m_fSamplePeriod;
            }
            
            yield return new WaitForSeconds( m_fLoopTime * 0.001f );
        }
    }
}
