using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {

	public float moveSpeed = 10.0f;
	public float rotationSpeed = 5.0f;

	public Transform character;

	private Vector3 initialPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float wheelMovement = Input.GetAxis ("Mouse ScrollWheel");

		if (wheelMovement > 0.0f) {
			this.transform.Translate (Vector3.forward * moveSpeed * Time.deltaTime);
		} else if (wheelMovement < 0.0f) {
			this.transform.Translate (Vector3.back * moveSpeed * Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.Mouse1)) {
			//transform.LookAt (character);
			//transform.RotateAround (character.position - Vector3.back * 5.0f, Vector3.up, Input.GetAxis ("Mouse X") * rotationSpeed);
		}
	}
}
