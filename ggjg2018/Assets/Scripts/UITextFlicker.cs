using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UITextFlicker : MonoBehaviour {

    public bool flickering = true;
    public float period = 3f;
    [Range(0.01f, 1f)]
    public float stepSize = 0.1f;
    [Range(0f, 1f)]
    public float minimumAlpha = 0.2f;

    private Text text;
    private Coroutine flicker;
    private bool isFlickering = false;

	// Use this for initialization
	void Start ()
    {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (flickering && !isFlickering)
        {
            flicker = StartCoroutine(runFlicker());
        }

        if (!flickering && isFlickering)
        {
            StopCoroutine(flicker);
            isFlickering = false;
        }
	}

    private IEnumerator runFlicker()
    {
        isFlickering = true;
        float x = 0;

        float a = 1 - minimumAlpha;
        float c = (a / 2) + minimumAlpha;

        while (flickering)
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
