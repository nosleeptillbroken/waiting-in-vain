using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Influencer : MonoBehaviour {

    public int owner;

    public int influence;

    public HashSet<GameTile> influencedTiles = new HashSet<GameTile>();

    public bool CanInfluence(GameTile tile)
    {
        return true;
    }

    public void OnTriggerEnter(Collider other)
    {
        GameTile gt = other.gameObject.GetComponent<GameTile>();
        if (gt != null)
        {
            influencedTiles.Add(gt);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        GameTile gt = other.gameObject.GetComponent<GameTile>();
        if (gt != null)
        {
            influencedTiles.Remove(gt);
        }
    }

    public void Update()
    {
        foreach (GameTile gt in influencedTiles)
        {
            gt.Influence(this);
        }
    }

    public void OnDisable()
    {
        influencedTiles.Clear();
    }
}
