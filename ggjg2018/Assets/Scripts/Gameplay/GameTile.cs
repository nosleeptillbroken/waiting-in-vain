using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    public static Color GetPlayerColor(int p)
    {
        const float less = 5f / 16f;
        const float half = 0.5f;

        switch (p)
        {
            default:
                return new Color(half, half, half);
                break;
            case 0:
                return new Color(half, less, less);
                break;
            case 1:
                return new Color(less, less, half);
                break;
            case 2:
                return new Color(half, half, less);
                break;
            case 3:
                return new Color(less, half, less);
                break;
        }
    }

    //
    [SerializeField] int owner = -1;
    // the owner of the tile when it is fully captured. -1 means neutral
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

    // control needed to become owner of the tile
    public float MaxControl = 256f;

    // current influence values for each player. influence determines the increase or decrease in control depending on the controlling player
    [SerializeField] float[] influence = new float[4]{0f,0f,0f,0f};

    //
    [SerializeField] int controllingPlayer = -1;
    // the player currently assuming control of the tile. -1 means neutral
    public int ControllingPlayer
    {
        get
        {
            return controllingPlayer;
        }
        set
        {
            controllingPlayer = (value < 4 && value >= 0) ? value : -1;
        }
    }

    // current amount of control
    [SerializeField] float control = 0f;

    bool isGenerated = false;

    HexCell cell = null;
    HexGrid grid = null;

    GameObject compoundCollider = null;
    [SerializeField] GameObject tower = null;

    public GameObject Tower
    {
        get
        {
            return tower;
        }
        set
        {
            if (value != tower && tower != null)
            {
                Destroy(tower);
                tower = null;
            }
            tower = value;
            if (tower != null)
            {
                tower.transform.position = transform.position;
            }
        }
    }

    public float GetInfluence(int i)
    {
        if (i < 0)
            return 0f;
        else
            return influence[i];
    }

    public HexCell GetCell()
    {
        return cell;
    }

    public float GetNetInfluence()
    {
        return GetInfluence(GetHighestInfluencer()) - GetInfluence(GetSecondHighestInfluencer());
    }

    public int GetHighestInfluencer()
    {
        int ret = -1;

        float highest = 0;
        for (int i = 0; i < influence.Length; i++)
        {
            if (influence[i] > highest)
            {
                highest = influence[i];
                ret = i;
            }
            else if (influence[i] >= highest)
            {
                // determine what to do when players are even
            }
        }

        return ret;
    }

    public int GetSecondHighestInfluencer()
    {
        int notRet = GetHighestInfluencer();
        int ret = -1;

        float highest = 0;
        for (int i = 0; i < influence.Length; i++)
        {
            if (influence[i] > highest && i != notRet)
            {
                highest = influence[i];
                ret = i;
            }
        }

        return ret;
    }

    public void OnGenerated(HexGrid grid)
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
    }

    public void OnSpawnPointSet(HexGrid grid, int player)
    {
        cell.Elevation = 0;
        Owner = player;
        ControllingPlayer = player;
        control = MaxControl;

        foreach (HexDirection d in HexDirection.GetValues(typeof(HexDirection)))
        {                    
            HexCell n = cell.GetNeighbor(d);
            if (n != null)
            {
                n.Elevation = 0;
            }
        }

        if (Tower == null)
        {
            GameObject towerPrefab = Resources.Load<GameObject>("SpawnTower");
            Tower = Instantiate<GameObject>(towerPrefab);
            Tower.GetComponent<Influencer>().owner = player;
        }
    }

    void Update()
    {
        if (isGenerated)
        {
            int primaryInfluencer = GetHighestInfluencer();
            int secondaryInfluencer = GetSecondHighestInfluencer();
            float netInfluence = GetNetInfluence();

            if (primaryInfluencer >= 0)
            {
                if (owner < 0) // if the tile is neutral
                {
                    if (ControllingPlayer < 0) // if the tile is completely neutral
                    {
                        control += netInfluence;
                        if (netInfluence > 0f)
                        {
                            ControllingPlayer = primaryInfluencer;
                        }
                    }
                    else if (ControllingPlayer == primaryInfluencer) // if a player is already taking control
                    {
                        control += netInfluence;
                    }
                    else
                    {
                        control -= netInfluence;
                    }
                }
                else // if the tile is owned by a player
                {                
                    if (owner == primaryInfluencer) // if the owner of the tile is not the primary influencer
                    {
                        control += netInfluence;
                    }
                    else
                    {
                        control -= netInfluence;
                    }
                }

                if (control >= MaxControl)
                {
                    control = MaxControl;
                    Owner = ControllingPlayer;
                }
                else if (control <= 0)
                {
                    control = 0;
                    Owner = -1;
                    ControllingPlayer = -1;
                    Tower = null;
                }
            }

            Transform display = transform.Find("Display");
            if (display != null)
            {
                display.GetComponent<MeshRenderer>().material.SetColor("_ColorMult", GetPlayerColor(controllingPlayer));
                display.GetComponent<MeshRenderer>().material.SetFloat("_CloudCutoff", control / MaxControl);
            }

            influence = new float[4]{0f,0f,0f,0f};
        }
    }

    // CHECK THE BELOW FUNCTIONS IF YOU HAVE ISSUES IN THE FUTURE WITH INFLUENCE NOT DISAPPEARING
    // MAY BE ISSUE WITH INFLUENCE VALUE BEING SUBTRACTED ON TRIGGER EXIT BEING DIFFERENT FROM THE INFLUENCE VALUE BEING ADDED ON TRIGGER ENTER
    // FIX BY KEEPING COUNT OF TOWERS CURRENTLY INFLUENCING TILES FOR EACH PLAYER, AND RESET INFLUENCE TO 0 IF NO TOWERS ARE CURRENTLY INFLUENCING

    float GetInfluenceFromInfluencer(Influencer influencer)
    {
        float adjustedInfluence = 0f;

        SphereCollider sphCol = influencer.gameObject.GetComponent<SphereCollider>();

        float dist = Vector3.Distance(sphCol.transform.position, transform.position);

        float sphRadius = Mathf.Max(sphCol.bounds.extents.z, sphCol.bounds.extents.y, sphCol.bounds.extents.z);

        float invDist = dist / sphRadius;

        adjustedInfluence = Mathf.Lerp(influencer.influence, 0f, invDist);

        if (influencer.CanInfluence(this))
        {
            return adjustedInfluence;
        }
        else
        {
            return 0f;
        }
    }

    public void Influence(Influencer inf)
    {
        Influencer influencer = inf.gameObject.GetComponent<Influencer>();
        if (influencer != null)
        {
            influence[influencer.owner] += GetInfluenceFromInfluencer(influencer);
        }
    }
}
