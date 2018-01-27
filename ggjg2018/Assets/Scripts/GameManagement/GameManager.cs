using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    //External References.
    SettingsToken settings;
    //HexGrid board;

    //Timer variables.
    private float timerValue;
    private float endgameTime;
    private float totalGameTime;

    //Tile dictionary.
    //Dictionary<string, GameTile> tileList = new Dictionary<string, HexGrid>();

    void Start ()
    {
        Initialize();
	}

    void Update ()
    {
		
	}

    private void Initialize()
    {
        settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<SettingsToken>();
        //board = GameObject.FindGameObjectWithTag("Board").GetComponent<HexGrid>();
    }

    /*
    public GameTile LookupTileData(string key)
    {
        GameTile tile = tileList.TryGetValue(key, out  GameTile value);
        return tile;
    }
    
    public void RegisterTileData(string key, GameTile reference)
    {
        tileList.add(key, reference);
    } 

    */




}
