using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : SingletonBehaviour<SoundSystem>
{
    [SerializeField] private AudioSource m_sourceBGM;

    public delegate void SoundSystemAction();
    public static event SoundSystemAction OnBGMPlay;
    public static event SoundSystemAction OnBGMStop;

    public AudioSource SourceBGM{ get{ return m_sourceBGM; }}

    public void OnClickPlay()
    {
        m_sourceBGM.Play();

        if( OnBGMPlay != null )
        {
            OnBGMPlay();
        }
    }

    public void OnClickStop()
    {
        m_sourceBGM.Stop();

        if( OnBGMStop != null )
        {
            OnBGMStop();
        }
    }
}
