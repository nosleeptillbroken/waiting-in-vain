using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SceneChangeWhenComplete : MonoBehaviour {

    private VideoPlayer myPlayer;


	// Use this for initialization
	void Start ()
    {
        myPlayer = GetComponent<VideoPlayer>();
        myPlayer.loopPointReached += EndReached;
    }
	
    private void EndReached(VideoPlayer vp)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
