using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltBoundingBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Bolt") {
			other.gameObject.GetComponent<Bolt> ().weapon.isAlreadyInFlight = false;
			Destroy (other.gameObject);
		}
	}
}
