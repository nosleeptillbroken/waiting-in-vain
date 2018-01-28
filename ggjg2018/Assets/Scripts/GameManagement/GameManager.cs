using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    //External References.
    SettingsToken settings;
    HexGrid board;

    //Timer variables.
    private float timerValue;
    private float endgameTime;
    private float totalGameTime;

    //Tile dictionary.
    Dictionary<string, GameTile> tileList = new Dictionary<string, GameTile>();

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
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<HexGrid>();
    }

    
    public GameTile LookupTileData(string key)
    {
        GameTile value;
        if (tileList.TryGetValue(key, out value))
        {
            return value;
        }
        else
        {
            Debug.Log("Couldn't find hex of value " + key);
            return null;
        }
    }
    
    public void RegisterTileData(string key, GameTile reference)
    {
        tileList.Add(key, reference);
        Debug.Log("Added tile: " + key);
    } 

    




}
