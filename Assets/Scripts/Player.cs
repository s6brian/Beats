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
        Vector3 scale = m_transform.localScale;// position;

        if(( m_beatListener.BeatMask & ( BeatType.DownBeat | BeatType.UpBeat | BeatType.OffBeat )) > 0 )//== BeatType.DownBeat )
        {
            scale.y = 4f;
        }

//        if(( m_beatListener.BeatMask & BeatType.UpBeat ) == BeatType.UpBeat )
//        {
//            pos.y = 0.5f;
//            m_transform.position = pos;
//        }

        if( scale.y > 1 )
        {
            scale.y -= 0.1f;
        }

        if( scale.y < 1 )
        {
            scale.y = 1;
        }

        m_transform.localScale = scale;//position = pos;
    }
}
