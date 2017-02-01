using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

	public float rotationVelocity = 3.0f;

	public bool pickedUp = false;

	public AudioSource audioSource;

	// Use this for initialization
	void Awake () {
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Rotate (new Vector3 (0.0f, 0.0f, 10.0f * rotationVelocity * Time.deltaTime));
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			this.pickedUp = true;
			AudioSource.PlayClipAtPoint (audioSource.clip, other.transform.position);

			this.gameObject.SetActive (false);
		}
	}
}
