using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryObject : MonoBehaviour 
{
	public float lifetime = 10.0f;
	float time = 0.0f;

	public TemporaryObject(float _lifetime)
	{
		lifetime = _lifetime;
	}

	// Use this for initialization
	void Start () 
	{
		Destroy(this.gameObject, lifetime);
	}
	
	// Update is called once per frame
	//void FixedUpdate () 
	//{
	//	time += Time.fixedDeltaTime;
	//	if(time > lifetime)
	//	{
	//		DestroyObject(this.gameObject);
	//	}
	//}
}
