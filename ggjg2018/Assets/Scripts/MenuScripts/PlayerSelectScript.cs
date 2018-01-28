using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectScript : MonoBehaviour
{
    private static Color blue;
    public Text[] text;
    bool readyCheck = false;
    private Color mc = blue;

    private void Awake()
    {
        //text = GetComponent<Text[]>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            readyCheck = true;
            text[1].color = mc;
            
        }
    }

}
