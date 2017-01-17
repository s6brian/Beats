using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayStopButton : MonoBehaviour
{
    [SerializeField] private GameObject m_objPlayIcon;
    [SerializeField] private GameObject m_objStopIcon;

    private SoundSystem m_soundSystem;
    private AudioSource m_BGMSource;

    protected void Start()
    {
        m_soundSystem = SoundSystem.Instance;
        m_BGMSource = m_soundSystem.SourceBGM;

        m_objPlayIcon.SetActive( !m_BGMSource.isPlaying );
        m_objStopIcon.SetActive( m_BGMSource.isPlaying );

        this.GetComponent<Button>().onClick.AddListener( OnClick );
    }

    private void OnClick()
    {
        m_objPlayIcon.SetActive( m_BGMSource.isPlaying );
        m_objStopIcon.SetActive( !m_BGMSource.isPlaying );

        if( m_BGMSource.isPlaying )
        {
            m_soundSystem.OnClickStop();
        }
        else
        {
            m_soundSystem.OnClickPlay();
        }
    }
}
