using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{

    public int width = 6;
    public int height = 6;

    public Color defaultColor = Color.white;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    HexCell[] cells;

    Canvas gridCanvas;
    HexMesh hexMesh;

    void Start()
    {
        Initialize();
        GenerateLevel();
        Refresh();
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        return GetCell(coordinates);
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        return cells[index];
    }

    public HexCoordinates GetPlayerStartCoordinate(int player)
    {
        switch (player)
        {
            case 0:
                return new HexCoordinates((-width/2)+2, height-2);
                break;
            case 1:
                return new HexCoordinates((width/2), height-2);
                break;
            case 2:
                return new HexCoordinates(1, 1);
                break;
            case 3:
                return new HexCoordinates(width-2, 1);
                break;
            default:
                Debug.LogError("Invalid player index (" + player.ToString() + ").", this);
                return new HexCoordinates(0, 0);
                break;
        }
    }

    public void Refresh()
    {
        hexMesh.Triangulate(cells);
    }

    void Initialize()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void GenerateLevel()
    {
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++, i++)
            {
                float iX = ((float)x + Random.Range(0,256)) / ((float)width);
                float iZ = ((float)z + Random.Range(0,256)) / ((float)height);

                cells[i].Elevation = Mathf.RoundToInt(Mathf.PerlinNoise(iX, iZ));

                GameTile gameTile = cells[i].GetComponent<GameTile>();
                gameTile.OnGenerated(this);
            }
        }

        for (int p = 0; p < 4; p++)
        {
            HexCell spawnCell = GetCell(GetPlayerStartCoordinate(p));
            GameTile gameTile = spawnCell.GetComponent<GameTile>();
            gameTile.OnSpawnPointSet(this, p);
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;
    }
}