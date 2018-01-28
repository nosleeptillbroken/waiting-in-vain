using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControl : MonoBehaviour {

    public void IsPlaying()
    {
        AudioSource audio = GetComponent<AudioSource>();
        
        if(audio.mute == false)
        {
            audio.mute = true;
        }
        else
        {
            audio.mute = false;
        }
    }

}
