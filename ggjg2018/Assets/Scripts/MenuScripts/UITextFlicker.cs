using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UITextFlicker : MonoBehaviour {

    public bool Flicker
    {
        get { return iSFlickering; }
        set
        {
            if (value && !iSFlickering)
            {
                flicker = StartCoroutine(runFlicker());
                iSFlickering = value;
            }

            if (iSFlickering && !value)
            {
                StopCoroutine(flicker);
                iSFlickering = value;
            }
        }
    }


    public float period = 3f;
    [Range(0.01f, 1f)]
    public float stepSize = 0.1f;
    [Range(0f, 1f)]
    public float minimumAlpha = 0.2f;

    private bool iSFlickering = true;
    private Text text;
    private Coroutine flicker;

	// Use this for initialization
	void Start ()
    {
        text = GetComponent<Text>();
        if (iSFlickering)
        {
            flicker = StartCoroutine(runFlicker());
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    private IEnumerator runFlicker()
    {
        iSFlickering = true;
        float x = 0;

        float a = 1 - minimumAlpha;
        float c = (a / 2) + minimumAlpha;

        while (iSFlickering)
        {
            float newAlpha = a * Mathf.Cos((period / Mathf.PI) * x) + c;
            Color newColor = text.color;
            newColor.a = newAlpha;
            text.color = newColor;

            x += stepSize;
            yield return new WaitForSeconds(stepSize);
        }
    }
}
