using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class BeatCounter : MonoBehaviour
{
    private AudioSource m_sourceBGM;
    private float m_fFeMultiplier;

    private const int SAMPLE_COUNT   = 1024;
    private const int SUBBAND_COUNT  = 64;
    private const int HISTORY_BUFFER = 43;

    private float[] m_samples          = new float[SAMPLE_COUNT ];
    private float[] m_subBands         = new float[SUBBAND_COUNT];
    private float[] m_aveSoundEnergies = new float[SUBBAND_COUNT];
    private float[] m_aveVariance      = new float[SUBBAND_COUNT];

    private float[,] m_eHistory = new float[SUBBAND_COUNT, HISTORY_BUFFER];

    public delegate void BeatCounterDelegate( BeatType p_beatType );
    public static event BeatCounterDelegate OnBeatNotify;

    public delegate void  FrequencyDelegate ();
    public static   event FrequencyDelegate OnSubBassNotify;
    public static   event FrequencyDelegate OnBassNotify;
    public static   event FrequencyDelegate OnLowMidNotify;
    public static   event FrequencyDelegate OnMidRangeNotify;
    public static   event FrequencyDelegate OnHighMidNotify;
    public static   event FrequencyDelegate OnPressenceNotify;
    public static   event FrequencyDelegate OnBrillianceNotify;

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
    }

    private void OnBGMPlay( double p_syncTime )
    {
        m_fFeMultiplier = ( float )SAMPLE_COUNT / ( float )m_sourceBGM.clip.frequency;
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

        int subBandsMax   = 0;
        int subBandsMin   = 0;
        float averageMult = 0f;

        for( int idx = 0; idx < SUBBAND_COUNT; ++idx )
        {
            m_subBands[idx] = 0;

            subBandsMin  = subBandsMax;
            subBandsMax  = Mathf.RoundToInt( 0.4785f * idx + 1.375f ); //=> wi = a * i + b
            averageMult  = ( float )subBandsMax / ( float )SAMPLE_COUNT;
            subBandsMax += subBandsMin;

            for( int jdx = subBandsMin; jdx < subBandsMax; ++jdx )
            {
                // if jdx is greater than samples count, loop back to 1st sample
                m_subBands[idx] += m_samples[jdx % SAMPLE_COUNT] * m_samples[jdx % SAMPLE_COUNT];
            }
            
            m_subBands[idx] *= averageMult;
        }
    }

    private void GetAverageSoundEnergy( int p_emptyHistoryBufferCount )
    {
        p_emptyHistoryBufferCount = Mathf.Max( 0, p_emptyHistoryBufferCount );

        float averageSoundEnergy = 0f;
        int seHistCol = HISTORY_BUFFER - p_emptyHistoryBufferCount;
        float averageMult = 1f / seHistCol;

        for( int idx = 0; idx < SUBBAND_COUNT; ++idx )
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

            // get sound energy average variance
//            for( int jdx = seHistCol - 1; jdx >= 0; --jdx )
//            {
//                float v = m_eHistory[idx, jdx] - m_aveSoundEnergies[idx];
//                m_aveVariance[idx] += v * v;
//            }
//
//            m_aveVariance[idx] *= averageMult;
        }
    }

    private bool BeatDetected( FrequencyRange p_feRange )
    {
        int minIdx = ( int )( FrequencyRangeValues.values[p_feRange][0] * m_fFeMultiplier );
        int maxIdx = ( int )( FrequencyRangeValues.values[p_feRange][1] * m_fFeMultiplier );

//        Debug.Log( "\nlisten to: " + p_feRange + ", minIdx: " + minIdx + ", maxIdx: " + maxIdx );
//        Debug.Log( "min frequency: " + FrequencyRangeValues.values[p_feRange][0] + ", max frequency: " + FrequencyRangeValues.values[p_feRange][1] + "\n" );

        float subbandAve = 0;
        float volumeAve = 0;

        for( int idx = minIdx; idx < maxIdx; ++idx )
        {
//            subbandAve += m_subBands[idx];
//            volumeAve  += m_aveSoundEnergies[idx];
            subbandAve += m_samples[idx];
        }

        subbandAve /= maxIdx - minIdx;
//        volumeAve  /= maxIdx - minIdx;
//
//        return( subbandAve > 1.5f * volumeAve );

//        Debug.Log( "vol: " + subbandAve );
        return subbandAve > 1E-3f;
    }

    // source: http://archive.gamedev.net/archive/reference/programming/features/beatdetection/
    private IEnumerator BeatDetection()
    {
        // used to determine how many previous sound energy was cached
        int emptyHistoryBufferCount = HISTORY_BUFFER;
        int subBandsLen = m_subBands.Length;
        float loopTime = 1f / ( float )emptyHistoryBufferCount;

        while( m_sourceBGM.isPlaying )
        {
            int beatCount = 0;
            m_sourceBGM.GetSpectrumData( m_samples, 0, FFTWindow.BlackmanHarris );
//            GetSubBands();
//            GetAverageSoundEnergy( --emptyHistoryBufferCount );

            /******************************************************************
                see: http://archive.gamedev.net/archive/reference/programming/features/beatdetection/
                     formula R13
                     
                f = ( i * fe ) / N
                f * N = i * fe
                i = f * ( N / fe ) => round off to get index
            ******************************************************************/

            if(    OnSubBassNotify != null
                && BeatDetected( FrequencyRange.SBass ))
            {
                OnSubBassNotify();
            }

            if(    OnBassNotify != null
                && BeatDetected( FrequencyRange.Bass ))
            {
                OnBassNotify();
            }

            if(    OnLowMidNotify != null
                && BeatDetected( FrequencyRange.LoMid ))
            {
                OnLowMidNotify();
            }

            if(    OnMidRangeNotify != null
                && BeatDetected( FrequencyRange.Mid ))
            {
                OnMidRangeNotify();
            }

            if(    OnHighMidNotify != null 
                && BeatDetected( FrequencyRange.HiMid ))
            {
                OnHighMidNotify();
            }

            if(    OnPressenceNotify != null 
                && BeatDetected( FrequencyRange.Pressence ))
            {
                OnPressenceNotify();
            }

            if(    OnBrillianceNotify != null 
                && BeatDetected( FrequencyRange.Brilliance ))
            {
                OnBrillianceNotify();
            }

            yield return new WaitForSeconds( loopTime );
        }
    }
}
