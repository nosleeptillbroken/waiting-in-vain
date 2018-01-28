using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRidOfMe : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(KillMe());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator KillMe()
    {
        yield return new WaitForSeconds(3);

        Destroy(gameObject);
    }
}
