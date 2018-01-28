using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayoutOptions : MonoBehaviour
{
    public GameObject[] layoutOptions;
    public int currentVisible = 0;

    public void setVisible(int idNum)
    {
        layoutOptions[currentVisible].SetActive(false);
        layoutOptions[idNum].SetActive(true);
        currentVisible = idNum;
    } 
}
