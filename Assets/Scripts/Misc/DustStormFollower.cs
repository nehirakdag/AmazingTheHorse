using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustStormFollower : MonoBehaviour {

	//public ParticleSystem dustStorm;
	public GameObject player;
	public bool playerInstantiated;

	private Vector3 startingPosition;
	private Vector3 centerPosition;

	// Use this for initialization
	void Start () {
		startingPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (playerInstantiated) {
			//this.transform.position = player.transform.position - startingPosition + new Vector3(0.0f, 5.0f, 0.0f);
		}
	}

	public void signalPlayerAlive(GameObject player) {
		this.player = player;
		playerInstantiated = true;
	}

	public void setCenterPosition(Vector3 center) {
		centerPosition = center;
		this.transform.position = centerPosition;
	}
}
