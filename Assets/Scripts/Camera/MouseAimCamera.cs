using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAimCamera : MonoBehaviour {

	public GameObject character;
	public float rotateSpeed = 5.0f;

	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = character.transform.position = this.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Input.GetKey (KeyCode.Mouse0)) {
			float horizontal = Input.GetAxis ("Mouse X") * rotateSpeed;
			character.transform.Rotate (0, horizontal, 0);

			float desiredAngle = character.transform.eulerAngles.y;
			Quaternion rotation = Quaternion.Euler (0, desiredAngle, 0);

			this.transform.position = character.transform.position - (rotation * offset);

			this.transform.LookAt (character.transform);

		}
	}
}
