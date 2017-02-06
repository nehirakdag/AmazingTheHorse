using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltShooter : MonoBehaviour {

	public GameObject boltPrefab;

	public bool isAlreadyInFlight = false;

	public Bolt shotFired;

	void Start() {
		this.transform.Translate (new Vector3 (0.0f, 0.0f, 3.0f));
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space) && !isAlreadyInFlight) {
			//Debug.Log ("Why");
			isAlreadyInFlight = true;

			Fire ();
		}
	}

	void Fire() {
		GameObject boltFired = Instantiate (boltPrefab);

		//boltFired.transform.parent = this.transform;
		boltFired.transform.position = this.transform.position;
		boltFired.transform.rotation = this.transform.rotation;

		shotFired = boltFired.GetComponent<Bolt> ();
		shotFired.weapon = this;

		//Debug.Log ("Bolt is at location: " + boltFired.transform.position);
	}
}
