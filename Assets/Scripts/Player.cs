using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class Player : MonoBehaviour
{
    private Transform m_transform;
    private BeatListener m_beatListener;

    protected void Start()
    {
        m_transform = this.transform;
        m_beatListener = this.GetComponent<BeatListener>();
    }

    protected void Update()
    {
        Vector3 pos = m_transform.position;

        if(( m_beatListener.BeatMask & ( BeatType.DownBeat | BeatType.UpBeat | BeatType.OffBeat )) > 0 )//== BeatType.DownBeat )
        {
            pos.y = 2f;
        }

//        if(( m_beatListener.BeatMask & BeatType.UpBeat ) == BeatType.UpBeat )
//        {
//            pos.y = 0.5f;
//            m_transform.position = pos;
//        }

        if( pos.y > 0 )
        {
            pos.y -= 0.1f;
        }

        if( pos.y < 0 )
        {
            pos.y = 0;
        }

        m_transform.position = pos;
    }
}
