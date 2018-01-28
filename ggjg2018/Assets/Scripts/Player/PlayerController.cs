using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour {

    //Script is only used for player gameplay controlling. Create a new script for playerUIcontrolling

    public int gamePlayerId = 0;

    public GameObject hexGridObject;
    private HexGrid hexGrid;





    //crucial player variables
    private Rewired.Player player { get { return ControllerAssigner.GetRewiredPlayer(gamePlayerId); } }
    public Vector3 CurrentPosition { get; private set; }
    public int TotalTiles { get; private set; }

    //input variables
    private bool isSelecting;
    private float verticalAxis;
    private float horizontalAxis;
    private bool isVerticalAxisInUse = false;
    private bool isHorizontalAxisInUse = false;

    void Awake ()
    {
        if (hexGridObject != null)
        {
            hexGrid = hexGridObject.GetComponent<HexGrid>();
        }
        else
        {
            Debug.Log("HexGridObject reference is null!");
        }

        CurrentPosition = new Vector3(hexGrid.GetPlayerStartCoordinate(gamePlayerId).X, hexGrid.GetPlayerStartCoordinate(gamePlayerId).Y, hexGrid.GetPlayerStartCoordinate(gamePlayerId).Z);
        Debug.Log("Current Position for player " + gamePlayerId + ": " +  CurrentPosition);
        
	}

    void GetInputs()
    {
        isSelecting = player.GetButtonDown("Select");
        verticalAxis = player.GetAxis("MoveVertical");
        horizontalAxis = player.GetAxis("MoveHorizontal");
    }

    void ProcessInputs()
    {

        if (horizontalAxis > 0.5f && verticalAxis > 0.5f && !isHorizontalAxisInUse && !isVerticalAxisInUse)
        {
            MoveNE();
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }

        if (horizontalAxis < -0.5f && verticalAxis > 0.5 && !isHorizontalAxisInUse && !isVerticalAxisInUse)
        {
            MoveNW();
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }

        if (horizontalAxis < -0.5f && verticalAxis < -0.5f && !isHorizontalAxisInUse && !isVerticalAxisInUse)
        {
            MoveSW();
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }

        if (horizontalAxis > 0.5f && verticalAxis < -0.5f && !isHorizontalAxisInUse && !isVerticalAxisInUse)
        {
            MoveSE();
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }

        if (horizontalAxis < -0.5f && !isHorizontalAxisInUse)
        {
            MoveW();
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }

        if (horizontalAxis > 0.5f && !isHorizontalAxisInUse)
        {
            MoveE();
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }

        

        if (horizontalAxis == 0)
            isHorizontalAxisInUse = false;

        if (verticalAxis == 0)
            isVerticalAxisInUse = false;

    }

    void MoveNE()
    {
        CurrentPosition = new Vector3(CurrentPosition.x, CurrentPosition.y - 1, CurrentPosition.z + 1);
        Debug.Log("Moving NE" + CurrentPosition);
    }

    void MoveNW()
    {
        CurrentPosition = new Vector3(CurrentPosition.x - 1, CurrentPosition.y, CurrentPosition.z + 1);
        Debug.Log("Moving NW" + CurrentPosition);
    }

    void MoveW()
    {
        CurrentPosition = new Vector3(CurrentPosition.x - 1, CurrentPosition.y, CurrentPosition.z);
        Debug.Log("Moving W" + CurrentPosition);
    }

    void MoveE()
    {
        CurrentPosition = new Vector3(CurrentPosition.x + 1, CurrentPosition.y, CurrentPosition.z);
        Debug.Log("Moving E" + CurrentPosition);
    }

    void MoveSE()
    {
        CurrentPosition = new Vector3(CurrentPosition.x + 1, CurrentPosition.y, CurrentPosition.z - 1);
        Debug.Log("Moving SE" + CurrentPosition);
    }

    void MoveSW()
    {
        CurrentPosition = new Vector3(CurrentPosition.x, CurrentPosition.y + 1, CurrentPosition.z - 1);
        Debug.Log("Moving SW" + CurrentPosition);
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
