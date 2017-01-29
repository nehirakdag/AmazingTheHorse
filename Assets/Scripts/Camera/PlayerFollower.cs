using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour {

	public Transform playerTransform;

	private Vector3 initialPosition;
	private Vector3 currentPosition;

	// Use this for initialization
	void Start () {
		initialPosition = playerTransform.position - this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		currentPosition = playerTransform.position - initialPosition;
		this.transform.position = currentPosition;
	}
}
