using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour {

    int owner = -1; // unowned
    public int Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = (value < 4 && value >= 0) ? value : -1;
        }
    }

    bool isGenerated = false;

    HexCell cell = null;
    HexGrid grid = null;

    GameObject compoundCollider = null;

    public Color GetPlayerColor(int player)
    {
        switch (player)
        {
            case 0:
                return Color.red;
                break;
            case 1:
                return Color.blue;
                break;
            case 2:
                return Color.yellow;
                break;
            case 3:
                return Color.green;
                break;
            default:
                return (grid != null) ? grid.defaultColor : Color.white;
                break;
        }
    }

    public HexCell GetCell()
    {
        return cell;
    }

    public void OnGenerated (HexGrid grid)
    {
        this.grid = grid;
        cell = grid.GetCell(transform.position);
        if (cell == null)
        {
            Debug.LogError("GameTile created with invalid matching cell; destroying object.", this);
            Destroy(gameObject);
            return;
        }

        isGenerated = true;
        gameObject.name = cell.coordinates.X.ToString() + "," + cell.coordinates.Y.ToString() + "," + cell.coordinates.Z.ToString();

        compoundCollider = transform.Find("Collision").gameObject;
        UpdateCollider();
    }

    public void OnSpawnPointSet (HexGrid grid, int player)
    {
        cell.Elevation = 0;
        Owner = player;
        cell.color = GetPlayerColor(Owner);

        foreach (HexDirection d in HexDirection.GetValues(typeof(HexDirection)))
        {                    
            HexCell n = cell.GetNeighbor(d);
            if (n != null)
            {
                n.Elevation = 0;
            }
        }
    }

    public void UpdateCollider()
    {
        float tileHeight = HexMetrics.elevationStep * 6f; // hardcoded - fix later
        compoundCollider.transform.localScale = new Vector3(HexMetrics.outerRadius, tileHeight, HexMetrics.outerRadius);
        compoundCollider.transform.position = new Vector3(compoundCollider.transform.position.x, transform.position.y - tileHeight/2, compoundCollider.transform.position.z);
        compoundCollider.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

	void Update () 
    {
        if (isGenerated)
        {
            cell.color = GetPlayerColor(Owner);
        }
	}

    void OnTriggerStay (Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
}
