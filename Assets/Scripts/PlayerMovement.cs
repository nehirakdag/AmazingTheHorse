using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float velocity = 5.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W)) {
			this.transform.Translate (Vector3.forward * velocity * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.A)) {
			this.transform.Translate (Vector3.left * velocity * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.S)) {
			this.transform.Translate (Vector3.back * velocity * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.D)) {
			this.transform.Translate (Vector3.right * velocity * Time.deltaTime);
		}
	}
}
