using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour {

    //Script is only used for player gameplay controlling. Create a new script for playerUIcontrolling
    //You can toggle a control map using //  rewiredPlayer.controllers.maps.SetMapsEnabled([value], [control map]);

    public int gamePlayerId = 0;
    private GameManager gameManager;

    public GameObject hexGridObject;
    private HexGrid hexGrid;
    private HexCell currentCell;

    // This is for debugging and should be removed
    private Color prevColor;
    private Color tempColor = Color.black;

    //Timing variables
    public float cooldownTime = 3.0f;
    private float placementCooldown;

    private float moveTime = 0;
    public float moveWait = 0.1f;

    public GameObject towerObj;

    //Power Metrics.
    public int currentPower;
    public int maxPower;

    //crucial player variables
    private Rewired.Player player { get { return ControllerAssigner.GetRewiredPlayer(gamePlayerId); } }
    public HexCoordinates CurrentPosition { get; private set; }
    public int TotalTiles { get; private set; }

    //input variables
    private bool isSelecting;
    private float verticalAxis;
    private float horizontalAxis;
    private bool isVerticalAxisInUse = false;
    private bool isHorizontalAxisInUse = false;

    void Start ()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        if (hexGridObject != null)
        {
            hexGrid = hexGridObject.GetComponent<HexGrid>();
        }
        else
        {
            Debug.Log("HexGridObject reference is null!");
        }

        //CurrentPosition = new Vector3(hexGrid.GetPlayerStartCoordinate(gamePlayerId).X, hexGrid.GetPlayerStartCoordinate(gamePlayerId).Y, hexGrid.GetPlayerStartCoordinate(gamePlayerId).Z);\
        CurrentPosition = hexGrid.GetPlayerStartCoordinate(gamePlayerId);
        currentCell = hexGrid.GetCell(CurrentPosition);
        prevColor = currentCell.color;
        Debug.Log("Current Position for player " + gamePlayerId + ": " +  CurrentPosition);
	}

    void GetInputs()
    {
        isSelecting = player.GetButtonDown("ActionA");

        verticalAxis = player.GetAxis("MoveVertical");
        horizontalAxis = player.GetAxis("MoveHorizontal");
    }

    


    void ProcessInputs()
    {

        if (isSelecting)
        {
            PlaceTower();
            Debug.Log("Placing Tower");
        }

        bool canMove = Time.time > moveTime;

        if (horizontalAxis >= 0.0f && verticalAxis > 0.5f && canMove)
        {
            //MoveNE();
            Move(HexDirection.NE);
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }

        else if (horizontalAxis < 0.0f && verticalAxis > 0.5 && canMove)
        {
            //MoveNW();
            Move(HexDirection.NW);
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }

        else if (horizontalAxis <= 0.0f && verticalAxis < -0.5f && canMove)
        {
            //MoveSW();
            Move(HexDirection.SW);
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }

        else if (horizontalAxis > 0.0f && verticalAxis < -0.5f && canMove)
        {
            //MoveSE();
            Move(HexDirection.SE);
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }

        else if (horizontalAxis < -0.5f && canMove)
        {
            //MoveW();
            Move(HexDirection.W);
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }

        else if (horizontalAxis > 0.5f && canMove)
        {
            //MoveE();
            Move(HexDirection.E);
            isVerticalAxisInUse = true;
            isHorizontalAxisInUse = true;
        }
    }

    private void Move(HexDirection direction)
    {
        currentCell.color = prevColor;
        currentCell = currentCell.GetNeighbor(direction) ?? currentCell;
        prevColor = currentCell.color;
        currentCell.color = tempColor;
        hexGrid.Refresh();

        moveTime = Time.time + moveWait;
        Debug.Log(direction.ToString() + CurrentPosition);
    }
    
    void PlaceTower()
    {
        //if (hasCooledDown)
        //{
        /*get current tile (use 'CurrentLocation') object
         *instantiate towerObj at said location
         * set newly instatiated tower prefab's transform.parent = to that of the tile
         * apply cooldown
         * hasCooledDown = false;  
         * currentCooldownTime = totalCooldowntime; 
         */

        GameTile currentTile = gameManager.LookupTileData(currentCell.coordinates.GetPositionKey());
        if(currentTile.Owner == gamePlayerId && currentTile.GetCell().Elevation == 0 && Time.time > placementCooldown)
        {
            placementCooldown = Time.time + cooldownTime;
            currentTile.Tower = Instantiate(towerObj, currentCell.transform);
        }

        
        //}
    }

    // Update is called once per frame
    void Update ()
    { 
        if(!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
           if(player == null) return;
        //
        //Debug.Log("Are we even fucking running?");
        GetInputs();
        ProcessInputs();

        //if (!hasCooledDown && currentCooldownTime >= 0f)
        //{
        //    currentCooldownTime -= Time.deltaTime;
        //}
        //else
        //{
        //    hasCooledDown = true;
        //}

        
	}
}
