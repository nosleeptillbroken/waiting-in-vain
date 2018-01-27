using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerIconControl : MonoBehaviour
{
	
    public Image playerCard;

    public Sprite[] playerIcons;

    void Start()
    {
        playerCard.sprite = playerIcons[0];
    }

    void Update()
    {
        if(Input.GetKeyDown("p"))
        {
            landGained();
        }
        if (Input.GetKeyDown("l"))
        {
            landLost();
        }
    }

    public void landGained()
    {
        playerCard.sprite = playerIcons[1];

        StartCoroutine(IconSwapDelay(2));
    }

    public void landLost()
    {
        playerCard.sprite = playerIcons[2];

        StartCoroutine(IconSwapDelay(2));
    }

    IEnumerator IconSwapDelay(float time)
    {
        yield return new WaitForSeconds(time);
        playerCard.sprite = playerIcons[0];
    }
}
