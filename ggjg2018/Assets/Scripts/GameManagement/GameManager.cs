using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { PLAY, PAUSE, GAMEOVER, SETUP };

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;  //Static instance of GameManager which allows it to be accessed by any other script.
    public BoardManager boardManager;   //responsible for making the board move, spawning enemies and the like.

    private Image healthbarBG;
    [SerializeField] private  Image healthBar;
    private static int playerScore = 0;
    private static int maxHealth = 5;
    private static int currentHealth = maxHealth;

    private Text scoreText; 

    public static GameState CurrentGameState; //playing by default



    //Awake is always called before any Start functions
    void Awake()
    {
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        //Check if instance already exists
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        //Set orientation of screen to landscape
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        CurrentGameState = GameState.SETUP;
        //Get a component reference to the attached BoardManager script
        boardManager = GetComponent<BoardManager>();


        InitGame();
    }

    //Initializes the game for each level.
    void InitGame()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        scoreText.text = "Score: ";

        healthBar = GameObject.Find("HealthBar").gameObject.GetComponent<Image>();
        healthBar.fillAmount = (float)currentHealth / maxHealth;

        boardManager.StartGame(); 
        //initializing enemies and making sure everything is where is needs to by/ resetting all values after a game
    }

    public static void IncrementScore(int val)
    {
        playerScore += val;
    }

    public static void DecrementHealth()
    {
        if (currentHealth >= 1)
            currentHealth--;
        //Debug.Log(currentHealth);
    }

    public static void IncrementHealth()
    {
        if (currentHealth <= 5)
            currentHealth++;
    }


    

    //Update is called every frame.
    void Update()
    {   
        if (CurrentGameState == GameState.PLAY)
        {
            //playing the game, moving the board, etc.
            scoreText.text = "Score:" + playerScore;
            healthBar.GetComponent<Image>().fillAmount = (float)currentHealth / maxHealth;

            if(Input.GetKeyDown(KeyCode.UpArrow))
                IncrementHealth();
            if(Input.GetKeyDown(KeyCode.DownArrow))
                DecrementHealth();


        }
        else if (CurrentGameState == GameState.GAMEOVER)
        {
            //loss condition, reset the game, etc.
        }

    }

    #region Getters
    public static GameState GetGameState() { return CurrentGameState; }

    public static int GetPlayerScore() { return playerScore; }
    #endregion

    #region Setters

#endregion

}
