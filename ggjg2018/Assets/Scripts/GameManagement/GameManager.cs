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

    //Totals Metrics.
    private int[] towersPlaced = new int[4];
    private int[] totalTiles = new int[4];
    private bool[] peakPower = new bool[4];
    public float gameOverCountdown = 10.0f;

    void Start ()
    {
        Initialize();
	}

    void Update ()
    {
	    bool gameOver = peakPower[0] && peakPower[1] && peakPower[2] && peakPower[3];
        if (gameOver)
        {
            StartCoroutine("GameOverTimeDawg");
        }
    }

    private void Initialize()
    {
        settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<SettingsToken>();
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<HexGrid>();
        for (int i = 0; i < 4; i++)
        {
            towersPlaced[i] = 0;
            totalTiles[i] = 0;
            peakPower[i] = false;
        }
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

    public int GetTowersPlaced(int index)
    {
        return towersPlaced[index];
    }

    public void ChangeTowersPlaced(int index, int value)
    {
        towersPlaced[index] += value;
    }

    public int GetTotalTiles(int index)
    {
        return totalTiles[index];
    }

    public void ChangeTotalTiles(int index, int value)
    {
        totalTiles[index] += value;
    }

    IEnumerator GameOverTimeDawg()
    {
        yield return new WaitForSeconds(gameOverCountdown);
        settings.towersPlaced = towersPlaced;
        settings.peakPower = peakPower;
        settings.totalTiles = totalTiles;
    }


}
