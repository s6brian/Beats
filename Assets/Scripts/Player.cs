using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    protected void OnEnable()
    {
        SoundSystem.OnBGMPlay += OnBGMPlay;
        SoundSystem.OnBGMStop += OnBGMStop;
    }

    protected void OnDisable()
    {
        SoundSystem.OnBGMPlay -= OnBGMPlay;
        SoundSystem.OnBGMStop -= OnBGMStop;
    }

    private void OnBGMPlay()
    {
        
    }

    private void OnBGMStop()
    {

    }
}
