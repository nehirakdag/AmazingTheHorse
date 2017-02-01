using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour {

	public float moveSpeed = 45.0f;
	public Rigidbody rb;

	public AudioSource audioSource;

	public BoltShooter weapon;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.AddForce (this.transform.forward * moveSpeed * 100.0f);
		audioSource = GetComponent<AudioSource> ();

		if (audioSource != null) {
			AudioSource.PlayClipAtPoint (audioSource.clip, transform.position);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag != "Player" && other.gameObject.tag != "BoundingBox" && other.gameObject.tag != "BoulderSpawner" && other.gameObject.tag != "Key") {
			weapon.isAlreadyInFlight = false;
			Destroy (this.gameObject);
		}
		if (other.gameObject.tag == "Boulder") {
			Destroy (other.gameObject);
		}
	}
}
