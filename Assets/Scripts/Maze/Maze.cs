using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

	public GameObject[] wallPrefabs;
	public GameObject floorPrefab;
	public GameObject cellPrefab;
	public GameObject caveDoorPrefab;

	public DustStormFollower dustStorm;

	public GameObject playerPrefab;

	public float wallLength = 1.0f;

	[Range(0.0f, 1.0f)]
	public float noTorchWallProbability = 0.8f;
	public IntVector2 size;

	public Cell[] cells;

	private Vector3 initialPosition;
	private GameObject walls;
	private GameObject floor;
	private GameObject cellsParent;

	private int currentCell = 0;
	private int totalCellNum;
	private int visitedCellNum = 0;
	private bool startedMazeCreation = false;
	private int currentNeighbor = 0;

	private List<int> cellStack = new List<int>();
	private int backingUp = 0;
	private int wallToBreak;

	/*[System.Serializable]
	public class Cell {
		public GameObject north;
		public GameObject east;
		public GameObject west;
		public GameObject south;
		public GameObject floor;

		public bool visited;
	}*/

	// Use this for initialization
	void Start () {
		walls = GameObject.FindGameObjectWithTag ("Walls");
		floor = GameObject.FindGameObjectWithTag ("Floor");
		cellsParent = GameObject.FindGameObjectWithTag ("Cells");

		CreateWalls ();
		CreateCells ();

		//StartCoroutine(CreateMaze ());
		CreateMaze();

		SetEntranceAndExit ();

		SetCharacter ();
	}

	void CreateWalls() {
		initialPosition = new Vector3 ((-size.x / 2) + wallLength / 2, 0.0f, (-size.z / 2) + wallLength / 2);

		Vector3 wallPosition = initialPosition;
		GameObject tempWall;

		int wallNum = 0;
		float randomNum;

		for (int i = 0; i < size.z; i++) {
			for (int j = 0; j <= size.x; j++) {
				randomNum = Random.Range (0.0f, 1.0f);
				wallPosition = new Vector3 (initialPosition.x + (j * wallLength) - wallLength / 2, 0.0f, initialPosition.z + (i * wallLength) - wallLength / 2);

				GameObject wallPrefabToUse = (randomNum < noTorchWallProbability) ? wallPrefabs [0] : wallPrefabs [1];

				tempWall = Instantiate (wallPrefabToUse, wallPosition, Quaternion.identity);

				tempWall.transform.parent = walls.transform;
				tempWall.transform.localScale = tempWall.transform.localScale * wallLength;
				tempWall.name = "Wall #" + wallNum;
				wallNum++;
			}
		}

		for (int i = 0; i <= size.z; i++) {
			for (int j = 0; j < size.x; j++) {
				randomNum = Random.Range (0.0f, 1.0f);
				wallPosition = new Vector3 (initialPosition.x + (j * wallLength), 0.0f, initialPosition.z + (i * wallLength) - wallLength);

				GameObject wallPrefabToUse = (randomNum < noTorchWallProbability) ? wallPrefabs [0] : wallPrefabs [1];

				tempWall = Instantiate (wallPrefabToUse, wallPosition, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;

				tempWall.transform.parent = walls.transform;
				tempWall.transform.localScale = tempWall.transform.localScale * wallLength;
				tempWall.name = "Wall #" + wallNum;
				wallNum++;
			}
		}
	}

	void CreateFloor() {

	}

	void CreateCells() {
		int wallNum = walls.transform.childCount;
		Wall[] allWalls = new Wall[wallNum];

		cells = new Cell[size.x * size.z];
		totalCellNum = size.x * size.z;

		int eastWestWallNum = 0;
		int childNum = 0;
		int termCount = 0;

		for (int i = 0; i < wallNum; i++) {
			allWalls [i] = walls.transform.GetChild (i).GetComponent<Wall>();
		}

		for (int cellNum = 0; cellNum < cells.Length; cellNum++) {
			if (termCount == size.x) {
				eastWestWallNum++;
				termCount = 0;
			}

			//cells [cellNum] = new Cell ();
			cells [cellNum] = Instantiate(cellPrefab).GetComponent<Cell>();
			cells [cellNum].transform.parent = cellsParent.transform;

			cells [cellNum].east = allWalls [eastWestWallNum];
			cells [cellNum].south = allWalls [childNum + (size.x + 1) * size.z];

			eastWestWallNum++;

			termCount++;
			childNum++;

			cells [cellNum].west = allWalls [eastWestWallNum];
			cells [cellNum].north = allWalls [(childNum + (size.x + 1) * size.z + size.x - 1)];

			cells [cellNum].name = "Cell " + cellNum;
			cells[cellNum].transform.localPosition = new Vector3
					(cells [cellNum].east.transform.position.x + wallLength / 2.0f, 0.0f,
					(cells [cellNum].east.transform.position.z));
			
			cells [cellNum].floor = Instantiate (floorPrefab);
			cells [cellNum].floor.transform.parent = floor.transform;
			cells [cellNum].floor.transform.position = cells [cellNum].transform.position;
			cells [cellNum].floor.transform.localScale = cells [cellNum].floor.transform.localScale * wallLength;
			cells [cellNum].floor.name = "Floor #" + cellNum;
		}
	}

	void CreateMaze() {
		while (visitedCellNum < totalCellNum) {
			if (startedMazeCreation) {
				GetNeighbors ();

				if (cells [currentNeighbor].visited == false && cells [currentCell].visited == true) {
					//yield return StartCoroutine(BreakWall ());
					BreakWall();
					cells [currentNeighbor].visited = true;
					visitedCellNum++;
					cellStack.Add (currentCell);
					currentCell = currentNeighbor;

					if (cellStack.Count > 0) {
						backingUp = cellStack.Count - 1;
					}
				}
			} else {
				currentCell = Random.Range (0, totalCellNum);
				cells [currentCell].visited = true;

				visitedCellNum++;
				startedMazeCreation = true;
			}

		}

		Debug.Log ("Finished");
	}

	void BreakWall() {
		//Debug.Log("Broke wall #" + wallToBreak);

		switch (wallToBreak) {
		case 1:
			Destroy (cells [currentCell].north.gameObject);
			Debug.Log("Broke wall #" + wallToBreak);
			break;
		case 2:
			Destroy (cells [currentCell].east.gameObject);
			Debug.Log("Broke wall #" + wallToBreak);
			break;
		case 3:
			Destroy (cells [currentCell].west.gameObject);
			Debug.Log("Broke wall #" + wallToBreak);
			break;
		case 4:
			Destroy (cells [currentCell].south.gameObject);
			Debug.Log("Broke wall #" + wallToBreak);
			break;
		default:
			break;
		}

		//yield return new WaitForSeconds (0.3f);
	}

	void GetNeighbors() {
		int length = 0;
		int[] neighbors = new int[4];

		int[] connectingWall = new int[4];
		int check = (((currentCell + 1) / size.x) - 1) * size.x + size.x;

		// west wall
		if (currentCell + 1 < totalCellNum && (currentCell + 1) != check) {
			if (cells [currentCell + 1].visited == false) {
				neighbors [length] = currentCell + 1;
				connectingWall [length] = 3;
				length++;
			}
		}

		// east wall
		if (currentCell - 1 >= 0 && currentCell != check) {
			if (cells [currentCell - 1].visited == false) {
				neighbors [length] = currentCell - 1;
				connectingWall [length] = 2;
				length++;
			}
		}

		// north wall
		if (currentCell + size.x < totalCellNum) {
			if (cells [currentCell + size.x].visited == false) {
				neighbors [length] = currentCell + size.x;
				connectingWall [length] = 1;
				length++;
			}
		}

		// south wall
		if (currentCell - size.x >= 0) {
			if (cells [currentCell - size.x].visited == false) {
				neighbors [length] = currentCell - size.x;
				connectingWall [length] = 4;
				length++;
			}
		}

		if (length != 0) {
			int randomNeighbor = Random.Range (0, length);
			currentNeighbor = neighbors [randomNeighbor];
			wallToBreak = connectingWall [randomNeighbor];
		} else {
			if (backingUp > 0) {
				currentCell = cellStack [backingUp];
				backingUp--;
			}
		}
	}

	void SetCharacter() {
		GameObject player = Instantiate (playerPrefab);
		player.transform.position = cells [0].transform.position;
		player.transform.Translate(new Vector3(0.0f, 3.0f, 0.0f));

		//dustStorm.signalPlayerAlive (player);
		//dustStorm.setCenterPosition(cells[size.x * size.z / 2].transform.position);
	}

	void SetEntranceAndExit() {
		GameObject entranceDoor = Instantiate (caveDoorPrefab);
		entranceDoor.transform.position = cells [0].east.gameObject.transform.position + new Vector3(0.5f, 0.0f, 0.0f);
		entranceDoor.name = "Entrance Door";

		GameObject exitDoor = Instantiate (caveDoorPrefab);
		exitDoor.transform.position = cells [cells.Length - 1].north.gameObject.transform.position + new Vector3(-0.1f, 0.0f, -0.6f);
		exitDoor.transform.Rotate (new Vector3 (0.0f, 90f, 0.0f));
		exitDoor.name = "Exit Door";

		exitDoor.GetComponentInChildren<ParticleSystem> ().gameObject.SetActive (false);
	}
}
