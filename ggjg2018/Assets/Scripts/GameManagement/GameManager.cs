using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //External References.
    SettingsToken settings;
    HexGrid board;
    public GameObject gameOverPanel;
    private ResultsPanel resultsPanelScript;
    public List<Text> totalTileText = new List<Text>();
    public List<Text> totalPowerText = new List<Text>();
    public List<PlayerController> players = new List<PlayerController>();

    //Timer variables.
    private float timerValue;
    private float endgameTime;
    private float totalGameTime;

    //Tile dictionary.
    Dictionary<string, GameTile> tileList = new Dictionary<string, GameTile>();

    //Totals Metrics.
    private int[] towersPlaced = { 0, 0, 0, 0 };
    private int[] totalTiles = { 0, 0, 0, 0 };
    //public bool[] peakPower = new bool[4];
    public List<bool> peakPower = new List<bool>();
    public float gameOverCountdown = 10.0f;

    private Coroutine finalCountdown = null;

    void Start ()
    {
        Initialize();
	}

    void Update ()
    {
        bool gameOver = players.Count > 0;
        foreach(bool thing in peakPower)
        {
            gameOver = gameOver && thing;
        }

        if (gameOver && finalCountdown == null)
        {
            finalCountdown = StartCoroutine("GameOverTimeDawg");
        }
        else if (!gameOver && finalCountdown != null)
        {
            StopCoroutine(finalCountdown);
            finalCountdown = null;
        }
    }

    private void Initialize()
    {
        settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<SettingsToken>();
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<HexGrid>();

        resultsPanelScript = gameOverPanel.GetComponent<ResultsPanel>();
        gameOverPanel.SetActive(false);

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject playerObject in playerObjects)
        {
            players.Add(playerObject.GetComponent<PlayerController>());
            peakPower.Add(false);
        }

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
        
        gameOverPanel.SetActive(true);
         for (int i = 0; i < 4; i++)
         {
             totalTileText[i].text = totalTiles[i].ToString();
             totalPowerText[i].text = towersPlaced[i].ToString();
         }

        StartCoroutine(WaitForTimeThenLoop(gameOverCountdown));

        //int highestScore = 0;
        //
        //int winner = 0;
        //for(int i = 0; i < totalTiles.Length; i++)
        //{
        //    highestScore = highestScore < totalTiles[i] ? totalTiles[i] : highestScore;
        //    winner = highestScore == totalTiles[i] ? i : winner;
        //
        //    resultsPanelScript.SetPlayerValues(i, totalTiles[i], towersPlaced[i]);
        //}
        //resultsPanelScript.SetWinningPlayer(winner);
    }

    IEnumerator WaitForTimeThenLoop(float time)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene(1);
    }


}
