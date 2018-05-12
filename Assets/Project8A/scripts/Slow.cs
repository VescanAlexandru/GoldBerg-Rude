using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : MonoBehaviour {
	Rigidbody body;
	Vector3 velocity;
	 
	void Start()
	{ 
		body = GameObject.Find ("Ball").GetComponent<Rigidbody> ();
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Throwable")||(other.gameObject.CompareTag("Magnet")))
		{
			Time.timeScale = 0.1f;


		} 

	}
	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.CompareTag ("Throwable") || (other.gameObject.CompareTag ("Magnet"))) {
			
			Time.timeScale = 1f;
		}
	}
}
		
