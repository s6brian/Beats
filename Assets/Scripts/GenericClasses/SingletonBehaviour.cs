using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour
{
    private static SingletonBehaviour<T> m_instance = null;
    public static SingletonBehaviour<T> Instance{ get{ return m_instance; }}

    protected void Awake()
    {
        if( m_instance != null )
        {
            // ensure that there's only one instance of class T per scene
            Destroy( this.gameObject );
            return;
        }

        m_instance = this;
    }
}
