using UnityEngine;

public class GameManager : MonoBehaviour
{
    //External References.
    SettingsToken settings;

    //Timer variables.
    private float timerValue;
    private float endgameTime;
    private float totalGameTime;

    void Start ()
    {
        Initialize();
	}

    void Update ()
    {
		
	}

    private void Initialize()
    {
        settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<SettingsToken>();
    }
}
