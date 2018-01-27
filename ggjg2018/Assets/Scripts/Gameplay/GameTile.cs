using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour {
    
    bool isGenerated = false;

    HexCell cell = null;

    GameObject compoundCollider = null;

    void OnGenerated (HexGrid grid)
    {
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

        }
	}

    void OnTriggerStay (Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
}
