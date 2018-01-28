using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour {

    public int gamePlayerId = 0;

    private Rewired.Player player { get { return ControllerAssigner.GetRewiredPlayer(gamePlayerId); } }

    //input variables
    private bool isSelecting;

    // Use this for initialization
    void Start () {
		
	}

    void GetInputs()
    {
        isSelecting = player.GetButtonDown("Select");
    }

    void ProcessInputs()
    {

    }
	
	// Update is called once per frame
	void Update ()
    { 
        if(!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
            if(player == null) return;

        GetInputs();
        ProcessInputs();
	}
}
