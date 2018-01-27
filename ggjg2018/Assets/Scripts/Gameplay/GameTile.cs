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
        compoundCollider.transform.localScale = new Vector3(HexMetrics.outerRadius, 1f, HexMetrics.outerRadius);
        compoundCollider.transform.localPosition = new Vector3(compoundCollider.transform.position.x, cell.Elevation - 0.5f, compoundCollider.transform.position.z);
        compoundCollider.transform.localRotation = Quaternion.Euler(0, 90, 0);
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
