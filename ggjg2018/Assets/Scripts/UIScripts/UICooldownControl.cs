using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICooldownControl : MonoBehaviour
{
    public Image cooldown1;
    public Image cooldown2;
    public float cooldownTime;
    bool isCooldown1;
    bool isCooldown2;

    void Update ()
    {
		if(Input.GetKeyDown("1"))
        {
            isCooldown1 = true;
        }
        if(Input.GetKeyDown("2"))
        {
            isCooldown2 = true;
        }

        if(isCooldown1)
        {
            cooldown1.fillAmount += cooldownTime * Time.deltaTime;
            if(cooldown1.fillAmount >= 1)
            {
                cooldown1.fillAmount = 0;
                isCooldown1 = false;
            }
        }
        if(isCooldown2)
        {
            cooldown2.fillAmount += cooldownTime * Time.deltaTime;
            if (cooldown2.fillAmount >= 1)
            {
                cooldown2.fillAmount = 0;
                isCooldown2 = false;
            }
        }
	}
}
