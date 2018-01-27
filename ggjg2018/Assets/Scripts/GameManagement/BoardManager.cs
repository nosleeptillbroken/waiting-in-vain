using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BoardManager : MonoBehaviour {

   [SerializeField] private GameObject layer1;
   [SerializeField] private GameObject layer2;
   [SerializeField] private GameObject layer3;
   [SerializeField] private GameObject sky;


    private float screenX = Screen.width/100;
    private float screenY = Screen.height/100;


    private GameState currentGameState;

    private int moveMult = 1;

    private static Vector3 foregroundSpeed;
     

    // Use this for initialization
    void Awake ()
    {
        currentGameState = GameManager.GetGameState();


        //order is background to foreground
        sky = Instantiate(sky, new Vector3(screenX * -2, 10.0f, 1f), Quaternion.identity);

        layer3 = Instantiate(layer3, new Vector3(screenX * -1, 2.5f, 0.0f), Quaternion.identity);       

        layer2 = Instantiate(layer2, new Vector3(screenX * -1, 0.3f, -1.0f), Quaternion.identity);

        layer1 = Instantiate(layer1, new Vector3(screenX * -1, -1.75f, -2.0f), Quaternion.identity);


        GameManager.CurrentGameState = GameState.PLAY;
        currentGameState = GameState.PLAY;

    }



    // Update is called once per frame
    void Update ()
    {
        if ( currentGameState == GameState.PLAY)
        {
            //Debug.Log("We're playing, bois");
            //sky.transform.Translate((Vector3.right * -1) * Time.deltaTime * moveMult); //doesn't change

            layer3.transform.Translate((Vector3.right * -1) * Time.deltaTime * moveMult);

            layer2.transform.Translate((Vector3.right * -1) * Time.deltaTime * (moveMult * 1.5f));

            foregroundSpeed = (Vector3.right * -1)  * (moveMult * 3f);

            layer1.transform.Translate(foregroundSpeed * Time.deltaTime);

            /*if (Input.GetKeyDown(KeyCode.Space))
                GameManager.IncrementScore(1);*/
        }
        else if (currentGameState == GameState.GAMEOVER)
        {
            Debug.Log("You're dead.");
        }
        else if (currentGameState == GameState.PAUSE)
        {
            Debug.Log("Paused.");
        }

		
	}


    public void StartGame()
    {
        
    }

    #region Getters

    public static Vector3 GetForeGroundSpeed()
    {
        return foregroundSpeed;
    }

    #endregion
}
