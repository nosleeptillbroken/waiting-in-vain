using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Influencer : MonoBehaviour {

    public int owner;

    public int influence;

    public bool CanInfluence(GameTile tile)
    {
        return true;
    }
}
