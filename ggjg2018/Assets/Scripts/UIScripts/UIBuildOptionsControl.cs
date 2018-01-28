using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildOptionsControl : MonoBehaviour
{
    public Text buildOption1;
    public Text buildOption2;

    public bool hasTower;

    void Start()
    {
        hasTower = true;    
    }

    void Update()
    {
        if(hasTower)
        {
            buildOption1.text = "A: Upgrade Range";
            buildOption2.text = "B: Upgrade Strength";
        }   
        else
        {
            buildOption1.text = "A: Build Area Tower";
            buildOption2.text = "B: Build Directed Tower";
        } 
    }
}
