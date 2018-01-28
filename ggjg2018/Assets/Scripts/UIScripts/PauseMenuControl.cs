using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PauseMenuControl : MonoBehaviour
{

    public GameObject pausePanel;
    public GameObject[] pauseButtons;
    public int counter = 0;

	void Start ()
    {
		
	}
	
	
	void Update ()
    {
		if(Input.GetKeyDown("q"))
        {
            Time.timeScale = 0.0f;
            pausePanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(pauseButtons[0]);
        }

        if(Input.GetKeyDown("w"))
        {
            counter++;
            EventSystem.current.SetSelectedGameObject(pauseButtons[counter]);
        }

        if (Input.GetKeyDown("w"))
        {
            counter--;
            EventSystem.current.SetSelectedGameObject(pauseButtons[counter]);
        }
    }

    public void resumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void quitGame()
    {
        Application.Quit();
    }

}
