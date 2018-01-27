using UnityEngine;

public class SettingsToken : MonoBehaviour
{
    public enum GameLength { Short, Medium, Long };
    public enum BoardSize { Small, Medium, Large };
    public enum PowerStart { Low, Medium, High };
    public enum UILayout { Vertical, Horizontal };

    public static SettingsToken instance = null;
    public GameLength gameLength;
    public float gameTime;    //seconds.
    public BoardSize gridSize;
    public int gridWidth;       //odd number.
    public int gridHeight;      //odd number.
    public PowerStart powerStart;
    public int startingPowerCap;

    public bool audioOn = true;
    public bool sfxOn = true;
    public UILayout uiLayout;

    //Singleton Instance.
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start ()
    {
        gameLength = GameLength.Medium;
        gameTime = 900.0f;
        gridSize = BoardSize.Medium;
        gridWidth = 21;
        gridHeight = 25;
        powerStart = PowerStart.Medium;
        startingPowerCap = 5;
        uiLayout = UILayout.Vertical;
	}
	
    public void SetAudio(bool b)
    {
        audioOn = b;
    }

    public void SetSFX(bool b)
    {
        sfxOn = b;
    }

    public void SetGameLength(GameLength len)
    {
        gameLength = len;
        switch (gameLength)
        {
            case GameLength.Short:
                gameTime = 600.0f;  //10 Minutes.
                break;

            case GameLength.Medium:
                gameTime = 900.0f;  //15 Minutes.
                break;

            case GameLength.Long:
                gameTime = 1200.0f; //20 Minutes.
                break;
        }
    }

    public void SetGridSize(BoardSize bs)
    {
        gridSize = bs;
        switch(gridSize)
        {
            case BoardSize.Small:
                gridWidth = 15;
                gridHeight = 19;
                break;

            case BoardSize.Medium:
                gridWidth = 21;
                gridHeight = 25;
                break;

            case BoardSize.Large:
                gridWidth = 25;
                gridHeight = 29;
                break;
        }
    }

    public void SetPowerCap(PowerStart pow)
    {
        powerStart = pow;
        switch(powerStart)
        {
            case PowerStart.Low:
                startingPowerCap = 3;
                break;

            case PowerStart.Medium:
                startingPowerCap = 5;
                break;

            case PowerStart.High:
                startingPowerCap = 7;
                break;
        }
    }

    public void SetUILayout(UILayout ui)
    {
        uiLayout = ui;
    }
}
