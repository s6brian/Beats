using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T: MonoBehaviour
{
    protected static T m_instance = null;
    public static T Instance{ get{ return m_instance; }}

    protected virtual void Awake()
    {
        if( m_instance != null && m_instance.GetInstanceID() != this.GetInstanceID() )
        {
            // ensure that there's only one instance of class T per scene
            Destroy( this.gameObject );
            return;
        }

        m_instance = this as T;
    }
}
