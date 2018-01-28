using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Influencer : MonoBehaviour {

    public static Color GetPlayerColor(int p)
    {
        const float none = 0f;
        const float less = 1f / 16f;

        switch (p)
        {
            default:
                return new Color(less, less, less);
                break;
            case 0:
                return new Color(less, none, none);
                break;
            case 1:
                return new Color(none, none, less);
                break;
            case 2:
                return new Color(less, less, none);
                break;
            case 3:
                return new Color(none, less, none);
                break;
        }
    }

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

        Transform quad = transform.Find("Quad");
        if (quad != null)
        {
            quad.GetComponent<MeshRenderer>().material.SetColor("_SonarRampColor", GetPlayerColor(owner));
        }
    }

    public void OnDisable()
    {
        influencedTiles.Clear();
    }
}
