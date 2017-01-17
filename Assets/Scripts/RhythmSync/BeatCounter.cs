using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class BeatCounter : MonoBehaviour
{
    [SerializeField] private BeatValue m_beatValue = BeatValue.QuarterBeat;
    [SerializeField] private BeatType m_beatType = BeatType.OnBeat;
    [SerializeField] private float m_loopTime = 30.0f;

    private AudioSource m_sourceBGM;

    private float m_nextBeatSample;
    private float m_samplePeriod;
//    private float m_sampleOffset;
    private float m_currentSample;

    protected void OnEnable()
    {
        SoundSystem.OnBGMPlay += OnBGMPlay;
        //SoundSystem.OnBGMStop += OnBGMStop;
    }

    protected void OnDisable()
    {
        SoundSystem.OnBGMPlay -= OnBGMPlay;
        //SoundSystem.OnBGMStop -= OnBGMStop;
    }

    protected void Start()
    {
        m_sourceBGM = SoundSystem.Instance.SourceBGM;

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
    }

    private void OnBGMPlay( double p_syncTime )
    {

    }

//    private void OnBGMStop( double p_syncTime )
//    {
//
//    }

    private IEnumerator BeatCheck()
    {
        while( m_sourceBGM.isPlaying )
        {
            m_currentSample = ( float )AudioSettings.dspTime * m_sourceBGM.clip.frequency;

            if( m_currentSample >= m_nextBeatSample )
            {
                // event call
                m_nextBeatSample += m_samplePeriod;
            }
            
            yield return new WaitForSeconds( m_loopTime * 0.001f );
        }
    }
}
