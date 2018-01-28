using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    [SerializeField] private float scrollSpeed = 300;

    enum Transition { None, SplashToMenu, MenuToSplash }
    [SerializeField] private RectTransform[] panels;
    private int currentPanel = 0;
    private Transition currentTransition = Transition.None;

	// Use this for initialization
	void Start ()
    {
        panels[1].anchoredPosition = new Vector2(0, Screen.height);
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(currentPanel == 0 && Input.GetKeyDown(KeyCode.A))
        {
            currentTransition = Transition.SplashToMenu;
            panels[1].anchoredPosition = new Vector2(0, Screen.height);
            currentPanel = 1;
        }

        if (currentPanel == 1 && Input.GetKeyDown(KeyCode.Backspace))
        {
            currentTransition = Transition.MenuToSplash;
            panels[0].anchoredPosition = new Vector2(0, Screen.height);
            currentPanel = 0;
        }

        switch (currentTransition)
        {
            case Transition.SplashToMenu:
                if (panels[0].anchoredPosition.y > -Screen.height)
                {
                    panels[0].anchoredPosition -= new Vector2(0, scrollSpeed) * Time.deltaTime;
                    panels[1].anchoredPosition -= new Vector2(0, scrollSpeed) * Time.deltaTime;
                }
                if (panels[1].anchoredPosition.y < Mathf.Epsilon)
                {
                    panels[1].anchoredPosition = new Vector2(0, 0);
                    currentTransition = Transition.None;
                }
                break;
            case Transition.MenuToSplash:
                if (panels[0].anchoredPosition.y > 0)
                {
                    panels[0].anchoredPosition -= new Vector2(0, scrollSpeed) * Time.deltaTime;
                    panels[1].anchoredPosition -= new Vector2(0, scrollSpeed) * Time.deltaTime;
                }
                else
                {
                    panels[0].anchoredPosition = new Vector2(0, 0);
                    currentTransition = Transition.None;
                }
                break;
        }
	}


}
