using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAlive : MonoBehaviour {

	public bool isAlive = true;
	public MazeManager manager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!isAlive) {
			manager.playerDied = true;
		}
	}
}
