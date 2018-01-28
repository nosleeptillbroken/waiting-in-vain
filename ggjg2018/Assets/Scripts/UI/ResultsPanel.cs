using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsPanel : MonoBehaviour {

    void Start()
    {
        SetPlayerValues(0, 10, 20);
        SetPlayerValues(1, 5, 9);
        SetPlayerValues(2, 12, 6);
        SetPlayerValues(3, 0, 3);
        SetWinningPlayer(0);
    }

    public void SetPlayerValues(int player, int numTiles, int totalPower)
    {
        Transform tiles = transform.Find("Player " + (player+1).ToString() + " Tiles");
        Transform power = transform.Find("Player " + (player+1).ToString() + " Power");

        tiles.GetComponent<Text>().text = numTiles.ToString();
        power.GetComponent<Text>().text = totalPower.ToString();
    }

    public void SetWinningPlayer(int player)
    {
        Transform title = transform.Find("Title");

        title.GetComponent<Text>().text = "Player " + (player + 1) + " Victory";
    }

}
