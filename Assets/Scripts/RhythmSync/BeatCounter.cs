using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class BeatCounter : MonoBehaviour
{
    [SerializeField] private BeatValue m_beatValue = BeatValue.QuarterBeat;
    [SerializeField] private BeatType m_beatType = BeatType.OnBeat;

    [Space]
    [SerializeField] private BeatListener[] m_arrayBeatListeners;

    private AudioSource m_sourceBGM;

    private float m_fNextBeatSample;
    private float m_fSamplePeriod;
    private float m_fCurrentSample;

    private float[]  m_samples          = new float[1024];
    private float[]  m_subBands         = new float[64];
    private float[,] m_eHistory         = new float[64, 43];// each sub band has 43 energy history
    private float[]  m_aveSoundEnergies = new float[64];

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
    }

    private void OnBGMPlay( double p_syncTime )
    {
        AudioClip currentClip = m_sourceBGM.clip;
        m_fNextBeatSample = ( float )p_syncTime * currentClip.frequency;
        StartCoroutine( "BeatDetection" );
    }

    private void GetSubBands()
    {
        /***********************************************************************************************
            size = ( n * b ) + ( a * (( n * ( n + 1 )) / 2 ))
            1024 = 64 b + ( a * ((64 * ( 64 + 1 )) / 2 ))
            1024 = 64 b + 2080 a
            --
            let C = .45

            1024 = 64 b + 2080 (.45)
            1024 - 936 = 64 b
            88 = 64b
            b = 1.375

            1024 = 64 (.45) + 2080 a
            1024 - 28.8 = 2080 a
            995.2 = 2080 a
            a = 0.4785
            --
            see: http://archive.gamedev.net/archive/reference/programming/features/beatdetection/
                 formula R12 for getting 'a' and 'b' 
                 that are needed in formula R10 to get wi {( a * i ) + b } or subBandsLen
        ***********************************************************************************************/

        int samplesCount  = m_samples.Length;
        int subBandsCount = m_subBands.Length;
        int subBandsMax   = 0;
        int subBandsMin   = 0;
        float averageMult = 0f;

        for( int idx = 0; idx < subBandsCount; ++idx )
        {
            m_subBands[idx] = 0;

            subBandsMin  = subBandsMax;
            subBandsMax  = Mathf.RoundToInt( 0.4785f * idx + 1.375f ); //=> wi = a * i + b
            averageMult  = ( float )subBandsMax / ( float )samplesCount;
            subBandsMax += subBandsMin;

            for( int jdx = subBandsMin; jdx < subBandsMax; ++jdx )
            {
                // if jdx is greater than samples count, loop back to 1st sample
                m_subBands[idx] += m_samples[jdx % samplesCount] * m_samples[jdx % samplesCount];
            }
            
            m_subBands[idx] *= averageMult;
        }
    }

    private void GetAverageSoundEnergy( int p_emptyHistoryBufferCount )
    {
        p_emptyHistoryBufferCount = Mathf.Max( 0, p_emptyHistoryBufferCount );

        float averageSoundEnergy = 0f;
        int seHistRow = m_eHistory.GetLength( 0 );
        int seHistCol = m_eHistory.GetLength( 1 ) - p_emptyHistoryBufferCount;
        float averageMult = 1f / seHistCol;

        for( int idx = 0; idx < seHistRow; ++idx )
        {
            m_aveSoundEnergies[idx] = 0;

            // shift history
            for( int jdx = seHistCol - 1; jdx > 0; --jdx )
            {
                m_eHistory        [idx, jdx]  = m_eHistory[idx, jdx - 1];
                m_aveSoundEnergies[idx     ] += m_eHistory[idx, jdx    ];
            }

            // insert new sound energy
            m_eHistory        [idx, 0]  = m_subBands[idx];
            m_aveSoundEnergies[idx   ] += m_subBands[idx];

            // get current average sound energy of the given subband
            m_aveSoundEnergies[idx] *= averageMult;
        }
    }

    // source: http://archive.gamedev.net/archive/reference/programming/features/beatdetection/
    private IEnumerator BeatDetection()
    {
        // used to determine how many previous sound energy was cached
        int emptyHistoryBufferCount = m_eHistory.GetLength( 1 );
        int subBandsLen = m_subBands.Length;
        float loopTime = 1f / ( float )emptyHistoryBufferCount;

        while( m_sourceBGM.isPlaying )
        {
            int beatCount = 0;
            m_sourceBGM.GetSpectrumData( m_samples, 0, FFTWindow.Rectangular );
            GetSubBands();
            GetAverageSoundEnergy( --emptyHistoryBufferCount );

//            for( int idx = 0; idx < subBandsLen; ++idx )
//            {
//                beatCount += ( m_subBands[idx] > 1.5f * m_aveSoundEnergies[idx] ) ? 1: 0;
//            }
//            Debug.Log( "beat count: <b>" + beatCount + "</b>" );

//            if( beatCount > subBandsLen * 0.0f )
            if( m_subBands[0] > 1.5f * m_aveSoundEnergies[0] )
            {
                foreach( BeatListener beatListener in m_arrayBeatListeners )
                {
                    beatListener.Notify( m_beatType );
                }
            }

            yield return new WaitForSeconds( loopTime );
        }
    }

//    private IEnumerator BeatCheck()
//    {
//        while( m_sourceBGM.isPlaying )
//        {
//            m_fCurrentSample = ( float )AudioSettings.dspTime * m_sourceBGM.clip.frequency;
//
//            if( m_fCurrentSample >= m_fNextBeatSample )
//            {
//                Debug.Log( "<b>curr sample: " + m_fCurrentSample + ", next sample: " + m_fNextBeatSample + "</b>" );
//
//                foreach( BeatListener beatListener in m_arrayBeatListeners )
//                {
//                    beatListener.Notify( m_beatType );
//                }
//                m_fNextBeatSample += m_fSamplePeriod;
//            }
//            else
//            {
//                Debug.Log( "curr sample: " + m_fCurrentSample + ", next sample: " + m_fNextBeatSample );
//            }
//            
//            yield return new WaitForSeconds( m_fLoopTime * 0.001f );
//        }
//    }
}
