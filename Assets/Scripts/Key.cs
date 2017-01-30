using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

	public float rotationVelocity = 3.0f;

	public bool pickedUp = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Rotate (new Vector3 (0.0f, 0.0f, 10.0f * rotationVelocity * Time.deltaTime));
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			this.pickedUp = true;

			this.gameObject.SetActive (false);
		}
	}
}
