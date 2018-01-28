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

    private GameObject playerSelection;

    // This is for debugging and should be removed
    //private Color prevColor;
    //private Color tempColor = Color.black;

    //Timing variables
    public float cooldownTime = 3.0f;
    private float placementCooldown;

    private float moveTime = 0;
    public float moveWait = 0.1f;

    public GameObject towerObj;

    //Power Metrics.
    public int currentPower;
    public int maxPower;
    public int powerDenom = 10;
    public int basePower = 5;

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

    public GameObject playerHudObject = null;

    void Start ()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        currentPower = 0;

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

        //prevColor = currentCell.color;

        playerSelection = Instantiate(Resources.Load<GameObject>("Player " + (gamePlayerId+1).ToString() + " Selection"), transform) as GameObject;
        playerSelection.transform.position = currentCell.transform.position + Vector3.up;

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
            Debug.Log("Placing Tower: " + PlaceTower());
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
        //currentCell.color = prevColor;
        currentCell = currentCell.GetNeighbor(direction) ?? currentCell;
        //prevColor = currentCell.color;
        //currentCell.color = tempColor;

        playerSelection.transform.position = currentCell.transform.position + Vector3.up;

        moveTime = Time.time + moveWait;
        Debug.Log(direction.ToString() + CurrentPosition);
    }
    
    bool PlaceTower()
    {
        GameTile currentTile = gameManager.LookupTileData(currentCell.coordinates.GetPositionKey());

        if (currentTile != null)
        {
            Debug.Log(currentTile.Tower == null);
            
            if (currentTile.Owner == gamePlayerId 
                    && currentTile.GetCell().Elevation == 0 
                        && Time.time > placementCooldown 
                            && currentPower < maxPower 
                                && currentTile.Tower == null)
            {
                placementCooldown = Time.time + cooldownTime;
                currentTile.Tower = Instantiate(towerObj, transform) as GameObject;
                currentPower++;
                if (currentPower == maxPower)
                {
                    gameManager.peakPower[gamePlayerId] = true; //This needs to change when we can remove towers.
                }
                return true;
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update ()
    { 
        if(!ReInput.isReady || player == null)
        {
            return;
        }
        
        GetInputs();
        ProcessInputs();

        if (playerHudObject != null)
        {
            UpdateHUD();
        }

        maxPower = (int)(gameManager.GetTotalTiles(gamePlayerId) / powerDenom) + basePower;
	}

    void UpdateHUD()
    {
        UICooldownControl cooldown = playerHudObject.GetComponent<UICooldownControl>();
        UIEnergyControl energy = playerHudObject.GetComponent<UIEnergyControl>();
        UIPlayerIconControl playerIcon = playerHudObject.GetComponent<UIPlayerIconControl>();
        UIBuildOptionsControl buildOptions = playerHudObject.GetComponent<UIBuildOptionsControl>();

        cooldown.cooldownTime = cooldownTime;

        energy.maxEnergy = maxPower;
        energy.usedEnergy = currentPower;

        buildOptions.hasTower = currentCell.GetComponent<GameTile>().Tower != null;
    }
}
