using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class BoulderSpawner : MonoBehaviour {

	public GameObject boulderPrefab;
	public TraversableCell startPosition;
	public TraversableCell endPosition;

	public AudioSource boulderNoise;
	public CameraShake shakeCamera;

	public bool isActive = false;

	public List<Boulder> rollingBoulders = new List<Boulder>();
	public MazeManager manager;

	// Use this for initialization
	void Start () {
		boulderNoise = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < rollingBoulders.Count; i++) {
			if (rollingBoulders[i].currentLocationCell.connectedNeighbors.Contains(endPosition)) {
				Destroy (rollingBoulders[i].gameObject);
				rollingBoulders.RemoveAt(i);
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" && !isActive) {
			Debug.Log ("Creating boulders!!");
			//shakeCamera.GetComponentInParent<HeadBob> ().enabled = false;
			shakeCamera.enabled = true;
			boulderNoise.Play ();
			isActive = true;
			InvokeRepeating ("SpawnBoulder", 0.5f, 3.0f);
			//SpawnBoulder();
		}
	}

	void SpawnBoulder() {
		Debug.Log ("Spawned Boulder!");
		GameObject boulder = Instantiate (boulderPrefab);
		boulder.tag = "Boulder";
		//boulder.transform.position = startPosition;
		boulder.GetComponent<Boulder>().currentLocationCell = startPosition;
		boulder.transform.parent = this.transform;

		boulder.GetComponent<Boulder> ().manager = manager;

		rollingBoulders.Add (boulder.GetComponent<Boulder> ());
	}
}
