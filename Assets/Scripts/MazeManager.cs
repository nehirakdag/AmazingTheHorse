using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class MazeManager : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject keyPrefab;

	public Transform keysHolder;

	public Maze maze;

	public GameObject exitDoor;

	[Range(0, 5)]
	public int numKeys = 3;

	private GameObject player;

	private List<Key> keys = new List<Key>();

	// Use this for initialization
	void Start () {
		StartCoroutine(GenerateMaze ());

		SetCharacter ();
		SetExitDoor ();
		PlaceKeys ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator GenerateMaze() {
		yield return StartCoroutine (maze.GenerateMaze ());
	}

	void SetCharacter() {
		player = Instantiate (playerPrefab);
		player.transform.position = maze.cells [0].transform.position;
		player.transform.Translate(new Vector3(-5.0f, 2.0f, -5.0f));

		player.GetComponentInChildren<Camera> ().transform.position += new Vector3 (0.0f, 2.0f, 0.0f);
	}

	void SetExitDoor() {
		DungeonExiter exiter = exitDoor.GetComponent<DungeonExiter> ();

		exiter.InitializeButtons ();
		exiter.character = player;
	}

	void PlaceKeys () {
		List<Vector3> keyLocations = maze.destroyedAlcoves;

		foreach (Vector3 keyLocation in keyLocations) {
			GameObject key = Instantiate (keyPrefab);

			key.transform.parent = keysHolder.transform;

			//Vector3 farthestWallDirection = GiveFarthestWallDirection (key);

			key.transform.position = keyLocation + new Vector3 (0.0f, 2.0f, 0.0f);// + farthestWallDirection * -8.0f;


			keys.Add (key.GetComponent<Key> ());
			keys [keys.Count - 1].name = "Key " + keys.Count;

			//Debug.Log (key.name + "'s farthest direction: " + farthestWallDirection);
		}
	}

	private Vector3 GiveFarthestWallDirection(GameObject key) {
		//Vector3 forward = key.transform.TransformDirection (Vector3.forward);
		//Vector3 backward = key.transform.TransformDirection (Vector3.back);

		//Vector3 left = key.transform.TransformDirection (Vector3.left);
		//Vector3 right = key.transform.TransformDirection (Vector3.right);

		RaycastHit[] hits = new RaycastHit[4];

		Ray forward = new Ray(transform.position, Vector3.forward);
		Ray backward = new Ray(transform.position, Vector3.back);
		Ray left = new Ray(transform.position, Vector3.left);
		Ray right = new Ray(transform.position, Vector3.right);

		if(Physics.Raycast(forward, out hits[0])) {
		}
		if(Physics.Raycast(backward, out hits[1])) {
		}
		if(Physics.Raycast(left, out hits[2])) {
		}
		if(Physics.Raycast(right, out hits[3])) {
		}

		for(int i = 0; i < 4; i++) {
			//Debug.Log ("Hit[" + i + "] distance = " + hits [i].distance);
		}

		Vector3 maxDistanceDirection = Vector3.forward;

		if (hits [1].distance > hits [0].distance) {
			maxDistanceDirection = Vector3.back;
		}

		if (hits [2].distance > hits [1].distance && hits [2].distance > hits [0].distance) {
			maxDistanceDirection = Vector3.left;
		}

		if (hits [3].distance > hits [2].distance && hits [3].distance > hits [1].distance && hits [3].distance > hits [0].distance) {
			maxDistanceDirection = Vector3.right;
		}

		return maxDistanceDirection;
	}
}
