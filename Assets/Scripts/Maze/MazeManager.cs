using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using WindowsInput;

public class MazeManager : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject keyPrefab;
	public GameObject boulderSpawnerPrefab;
	public GameObject traversableCellPrefab;

	public GameObject reloadLevelPanel;
	public Button reloadLevelButton;

	public Transform keysHolder;

	public Maze maze;

	public GameObject enterDoor;
	public GameObject exitDoor;

	public AudioClip[] clips = new AudioClip[3];

	public bool playerDied = false;

	public Texture2D cursorTexture;

	private BoulderSpawner boulderSpawner;
	private DungeonExiter exiter;
	private GameObject player;

	public ParticleSystem teleportAura;

	private List<Key> keys = new List<Key>();

	private List<TraversableCell> traversableCells = new List<TraversableCell>();

	private AudioSource audioSource;

	// Use this for initialization
	void Start () {
		StartCoroutine (GenerateMaze ());

		if (!maze.builtMazeSuccessfully) {
			ReloadLevel ();
		}

		DivideCells ();
		LabelAlcoves ();

		PlaceKeys ();

		PlaceBoulderSpawner ();

		SetCharacter ();
		SetExitDoor ();
		boulderSpawner.shakeCamera = player.GetComponentInChildren<CameraShake> ();
		boulderSpawner.shakeCamera.enabled = false;

		reloadLevelButton.onClick.AddListener (ReloadGame);

		audioSource = GetComponent<AudioSource> ();
		//AudioSource.PlayClipAtPoint (audioSource.clip, transform.position);
		audioSource.Play();
	}

	// Update is called once per frame
	void Update () {
		if (!exiter.isActivated && AllKeysPickedUp ()) {
			exiter.isActivated = true;
			teleportAura.gameObject.SetActive(true);
		}

		if (playerDied) {
			LaunchReloadPanel ();
		}

	}

	void FixedUpdate() {
		if (boulderSpawner.isActive) {
			foreach (Boulder boulder in boulderSpawner.rollingBoulders) {
				TraversableCell currentLocation = boulder.currentLocationCell;

				if (boulder != null) {
					foreach (TraversableCell neighbor in currentLocation.connectedNeighbors) {
						if (neighbor.navigatable) {
							//Debug.Log ("Rolling boulder towards " + neighbor.name);
							Vector3 targetRollingPoint = neighbor.transform.position;
							targetRollingPoint.y = 0;

							//Debug.Log ("Difference is: " + (boulder.transform.position - targetRollingPoint).magnitude);
							if ((boulder.transform.position - targetRollingPoint).magnitude > 3.0f) {
								boulder.MoveTowards (targetRollingPoint);
							} else {
								boulder.currentLocationCell = neighbor;
							}
							//if (boulder.RollTowards (neighbor.transform.localPosition /*+ new Vector3 (0.0f, 2.0f, 0.0f)*/)) {
							//	boulder.currentLocationCell = neighbor;
							//boulder.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
							//boulder.transform.position = neighbor.transform.position;
							//}
						}
					}
				}
			}
		}
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
		exiter = exitDoor.GetComponent<DungeonExiter> ();

		exiter.InitializeButtons ();
		exiter.character = player;
		exiter.isActivated = false;
	}

	void PlaceBoulderSpawner() {
		GameObject boulderSpawnerGameObject = Instantiate (boulderSpawnerPrefab);
		boulderSpawnerGameObject.transform.position = maze.boulderSpawnerLocation;

		boulderSpawnerGameObject.tag = "BoulderSpawner";

		boulderSpawner = boulderSpawnerGameObject.GetComponent<BoulderSpawner> ();

		if (maze.rotateSpawner90Degrees) {
			boulderSpawner.transform.Rotate(new Vector3 (0.0f, 0.0f, 90.0f));
		}

		boulderSpawner.startPosition = traversableCells[2];
		boulderSpawner.endPosition = traversableCells [0];
		boulderSpawner.manager = this;
	}

	void PlaceKeys () {
		List<Vector3> keyLocations = maze.destroyedAlcoves;

		foreach (Vector3 keyLocation in keyLocations) {
			GameObject key = Instantiate (keyPrefab);

			key.transform.parent = keysHolder.transform;

			key.transform.position = keyLocation + new Vector3 (0.0f, 2.0f, 0.0f);// + farthestWallDirection * -8.0f;
			key.gameObject.tag = "Key";


			keys.Add (key.GetComponent<Key> ());
			keys [keys.Count - 1].name = "Key " + keys.Count;
			key.GetComponent<Key> ().audioSource.clip = clips [keys.Count - 1];
		}
	}

	private bool AllKeysPickedUp() {
		foreach(Key key in keys) {
			if (!key.pickedUp) {
				return false;
			}
		}

		return true;
	}

	private void DivideCells() {
		int cellID = 0;

		foreach (Cell cell in maze.cells) {
			GameObject topLeftCell = Instantiate (traversableCellPrefab);
			GameObject topRightCell = Instantiate (traversableCellPrefab);
			GameObject bottomLeftCell = Instantiate (traversableCellPrefab);
			GameObject bottomRightCell = Instantiate (traversableCellPrefab);

			topLeftCell.transform.parent = cell.transform;
			topRightCell.transform.parent = cell.transform;
			bottomLeftCell.transform.parent = cell.transform;
			bottomRightCell.transform.parent = cell.transform;

			topLeftCell.transform.localScale *= maze.wallLength;
			topRightCell.transform.localScale *= maze.wallLength;
			bottomLeftCell.transform.localScale *= maze.wallLength;
			bottomRightCell.transform.localScale *= maze.wallLength;

			topLeftCell.transform.localPosition = new Vector3 (-maze.wallLength / 4.0f, 0.0f, maze.wallLength / 4.0f);
			topRightCell.transform.localPosition = new Vector3 (maze.wallLength / 4.0f, 0.0f, maze.wallLength / 4.0f);
			bottomLeftCell.transform.localPosition = new Vector3 (-maze.wallLength / 4.0f, 0.0f, -maze.wallLength / 4.0f);
			bottomRightCell.transform.localPosition = new Vector3 (maze.wallLength / 4.0f, 0.0f, -maze.wallLength / 4.0f);

			topLeftCell.GetComponent<TraversableCell> ().parent = cell;
			topLeftCell.GetComponent<TraversableCell> ().id = cellID;
			topLeftCell.name = "Traversable Cell " + cellID;
			cellID++;

			topRightCell.GetComponent<TraversableCell> ().parent = cell;
			topRightCell.GetComponent<TraversableCell> ().id = cellID;
			topRightCell.name = "Traversable Cell " + cellID;
			cellID++;

			bottomLeftCell.GetComponent<TraversableCell> ().parent = cell;
			bottomLeftCell.GetComponent<TraversableCell> ().id = cellID;
			bottomLeftCell.name = "Traversable Cell " + cellID;
			cellID++;

			bottomRightCell.GetComponent<TraversableCell> ().parent = cell;
			bottomRightCell.GetComponent<TraversableCell> ().id = cellID;
			bottomRightCell.name = "Traversable Cell " + cellID;
			cellID++;

			traversableCells.Add (topLeftCell.GetComponent<TraversableCell> ());
			traversableCells.Add (topRightCell.GetComponent<TraversableCell> ());
			traversableCells.Add (bottomLeftCell.GetComponent<TraversableCell> ());
			traversableCells.Add (bottomRightCell.GetComponent<TraversableCell> ());

			cell.childCells.Add (topLeftCell.GetComponent<TraversableCell> ());
			cell.childCells.Add (topRightCell.GetComponent<TraversableCell> ());
			cell.childCells.Add (bottomLeftCell.GetComponent<TraversableCell> ());
			cell.childCells.Add (bottomRightCell.GetComponent<TraversableCell> ());

			cell.floor.gameObject.SetActive (false);
		}

		Queue<TraversableCell> cellsQueue = new Queue<TraversableCell> ();

		cellsQueue.Enqueue (traversableCells [2]);

		while (cellsQueue.Count != 0) {
			TraversableCell currentCell = cellsQueue.Dequeue ();
			currentCell.visited = true;
			currentCell.navigatable = true;

			//Debug.Log ("Popped " + currentCell.name);

			foreach (Vector3 direction in MazeDirections.directions) {
				RaycastHit hit;

				if (Physics.Raycast (currentCell.transform.position + new Vector3(0.0f, 2.0f, 0.0f), direction, out hit, 5.0f) && hit.collider.gameObject.tag == "Wall") {
					//Debug.Log (currentCell.name + " hit " + hit.collider.gameObject.name + " in direction : " + MazeDirections.GetDirectionName(direction));

					if(hit.collider.gameObject.activeInHierarchy) {
					//	Debug.Log (hit.collider.gameObject.name + " is active in the hierarchy");
					}
				} else {
					//Debug.Log ("No wall in direction: " + MazeDirections.GetDirectionName(direction) + " for cell " + currentCell.name);
					if (Physics.Raycast (currentCell.transform.position + new Vector3(0.0f, 2.0f, 0.0f) + direction * 10.0f, Vector3.down, out hit)) {
						TraversableCell cellHit = hit.collider.GetComponent<Collider>().gameObject.GetComponentInParent<TraversableCell> ();

						if (!cellHit.visited) {	
							currentCell.connectedNeighbors.Add (cellHit);
							cellsQueue.Enqueue (cellHit);
						}
					}
				}
			}
		}
	}

	void LabelAlcoves() {
		foreach (Vector3 alcoveTileLocation in maze.alcoveTileLocations) {
			RaycastHit hit;
			Debug.DrawLine (alcoveTileLocation, alcoveTileLocation + new Vector3 (0.0f, -2.0f, 0.0f));

			if (Physics.Raycast (alcoveTileLocation, Vector3.down, out hit)) {
				TraversableCell cellHit = hit.collider.GetComponent<Collider>().gameObject.GetComponentInParent<TraversableCell> ();

				if (cellHit != null) {
					//cellHit.gameObject.SetActive (false);
					cellHit.navigatable = false;
				}
			}
		}
	}

	public void LaunchReloadPanel() {
		reloadLevelPanel.SetActive (true);
		player.GetComponent<RigidbodyFirstPersonController> ().enabled = false;
		Cursor.visible = true;
		Cursor.SetCursor (cursorTexture, Vector2.zero, CursorMode.Auto);
		InputSimulator.SimulateKeyPress (VirtualKeyCode.ESCAPE);
	}
		
	void ReloadGame() {
		SceneManager.LoadScene ("MainScene", LoadSceneMode.Single);
	}

	void ReloadLevel() {
		SceneManager.LoadScene ("MazeScene", LoadSceneMode.Single);
	}


}
