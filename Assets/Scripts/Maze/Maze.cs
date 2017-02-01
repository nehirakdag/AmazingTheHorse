using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

	public GameObject[] wallPrefabs;
	public GameObject floorPrefab;
	public GameObject cellPrefab;
	public GameObject caveDoorPrefab;

	public bool builtMazeSuccessfully = false;

	public DustStormFollower dustStorm;

	public GameObject playerPrefab;

	public float wallLength = 1.0f;

	[Range(0, 5)]
	public int numAlcoves = 3;
	private int alcovesLeftToBuild;

	[Range(0.0f, 1.0f)]
	public float noTorchWallProbability = 0.8f;
	public IntVector2 size;

	public Cell[] cells;
	private Wall[] allWalls;

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
	private int wallNum = 0;

	private Dictionary<int, int> movements = new Dictionary<int, int>();
	private List<Wall> unicursalWalls = new List<Wall>();

	public List<Vector3> destroyedAlcoves = new List<Vector3>();
	public List<Vector3> alcoveTileLocations = new List<Vector3> ();
	public Vector3 boulderSpawnerLocation;
	public bool rotateSpawner90Degrees = false;

	// Use this for initialization
	void Start () {
		
	}

	void DumpMaze() {
		foreach (Wall wall in allWalls) {
			Destroy (wall.gameObject);
		}

		allWalls = null;

		foreach (Cell cell in cells) {
			Destroy (cell.gameObject);
		}

		cells = null;

		cellStack.Clear ();
		movements.Clear ();

		currentCell = 0;
		totalCellNum = 0;
		visitedCellNum = 0;
		startedMazeCreation = false;
		currentNeighbor = 0;

		//List<int> cellStack = new List<int>();
		backingUp = 0;
		wallToBreak = 0;
		wallNum = 0;

		alcovesLeftToBuild = numAlcoves;

		for (int i = 0; i < floor.transform.childCount; i++) {
			Destroy(floor.transform.GetChild(i).gameObject);
		}

		foreach (Wall wall in unicursalWalls) {
			Destroy (wall.gameObject);
		}

		for (int i = 0; i < walls.transform.childCount; i++) {
			Destroy(walls.transform.GetChild(i).gameObject);
		}

		unicursalWalls.Clear ();
		destroyedAlcoves.Clear ();

		//private Dictionary<int, int> movements = new Dictionary<int, int>();
		//private List<Wall> unicursalWalls = new List<Wall>();
	}

	public IEnumerator GenerateMaze() {
		walls = GameObject.FindGameObjectWithTag ("Walls");
		floor = GameObject.FindGameObjectWithTag ("Floor");
		cellsParent = GameObject.FindGameObjectWithTag ("Cells");

		CreateWalls ();
		CreateCells ();

		//StartCoroutine(CreateMaze ());
		CreateMaze();
		FixEntranceWallOrientation ();

		//SetCharacter ();

		//SolveMaze ();
		CreateAlcoves();

		if (alcovesLeftToBuild != 0) {
			Debug.Log ("Dumping maze! Will recreate it!");
			DumpMaze ();
			//yield return StartCoroutine(GenerateMaze ());
		} else {
			builtMazeSuccessfully = true;
		}

		yield return null;
	}

	void CreateWalls() {
		initialPosition = new Vector3 ((-size.x / 2) + wallLength / 2, 0.0f, (-size.z / 2) + wallLength / 2);

		Vector3 wallPosition = initialPosition;
		GameObject tempWall;

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

		int numberOfTotalWalls = walls.transform.childCount;
		allWalls = new Wall[numberOfTotalWalls];

		for (int i = 0; i < numberOfTotalWalls; i++) {
			allWalls [i] = walls.transform.GetChild (i).GetComponent<Wall>();
			allWalls [i].id = i;
		}
	}

	void PopulateWallNeighborsList() {
		int dimensionChange = (size.x + 1) * size.z;

		foreach (Cell cell in cells) {
			Wall eastWall = allWalls[cell.east.GetComponent<Wall> ().id];
			Wall westWall = allWalls[cell.west.GetComponent<Wall> ().id];

			Wall northWall = allWalls[cell.north.GetComponent<Wall> ().id];
			Wall southWall = allWalls[cell.south.GetComponent<Wall> ().id];

			foreach (Cell neighbor in eastWall.cells) {
				if (!eastWall.connectingWalls.Contains (allWalls [neighbor.north.GetComponent<Wall> ().id])) {
					eastWall.connectingWalls.Add (allWalls [neighbor.north.GetComponent<Wall> ().id]);
				}
				if (!eastWall.connectingWalls.Contains (allWalls [neighbor.south.GetComponent<Wall> ().id])) {
					eastWall.connectingWalls.Add (allWalls [neighbor.south.GetComponent<Wall> ().id]);
				}
			}

			foreach (Cell neighbor in westWall.cells) {
				if (!westWall.connectingWalls.Contains (allWalls [neighbor.north.GetComponent<Wall> ().id])) {
					westWall.connectingWalls.Add (allWalls [neighbor.north.GetComponent<Wall> ().id]);
				}
				if (!westWall.connectingWalls.Contains (allWalls [neighbor.south.GetComponent<Wall> ().id])) {
					westWall.connectingWalls.Add (allWalls [neighbor.south.GetComponent<Wall> ().id]);
				}
			}

			foreach (Cell neighbor in northWall.cells) {
				if (!northWall.connectingWalls.Contains (allWalls [neighbor.east.GetComponent<Wall> ().id])) {
					northWall.connectingWalls.Add (allWalls [neighbor.east.GetComponent<Wall> ().id]);
				}
				if (!northWall.connectingWalls.Contains (allWalls [neighbor.west.GetComponent<Wall> ().id])) {
					northWall.connectingWalls.Add (allWalls [neighbor.west.GetComponent<Wall> ().id]);
				}
			}

			foreach (Cell neighbor in southWall.cells) {
				if (!southWall.connectingWalls.Contains (allWalls [neighbor.east.GetComponent<Wall> ().id])) {
					southWall.connectingWalls.Add (allWalls [neighbor.east.GetComponent<Wall> ().id]);
				}
				if (!southWall.connectingWalls.Contains (allWalls [neighbor.west.GetComponent<Wall> ().id])) {
					southWall.connectingWalls.Add (allWalls [neighbor.west.GetComponent<Wall> ().id]);
				}
			}
		}
	}

	void CreateCells() {

		cells = new Cell[size.x * size.z];
		totalCellNum = size.x * size.z;

		int eastWestWallNum = 0;
		int childNum = 0;
		int termCount = 0;

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

			cells [cellNum].east.cells.Add (cells [cellNum]);
			cells [cellNum].west.cells.Add (cells [cellNum]);
			cells [cellNum].north.cells.Add (cells [cellNum]);
			cells [cellNum].south.cells.Add (cells [cellNum]);

			cells [cellNum].id = cellNum;
			cells [cellNum].normalWalls.Add (cells [cellNum].east);
			cells [cellNum].normalWalls.Add (cells [cellNum].south);
			cells [cellNum].normalWalls.Add (cells [cellNum].west);
			cells [cellNum].normalWalls.Add (cells [cellNum].north);
			
			cells [cellNum].floor = Instantiate (floorPrefab);
			cells [cellNum].floor.transform.parent = floor.transform;
			cells [cellNum].floor.transform.position = cells [cellNum].transform.position;
			cells [cellNum].floor.transform.localScale = cells [cellNum].floor.transform.localScale * wallLength;
			cells [cellNum].floor.name = "Floor #" + cellNum;
		}

		PopulateCellNeighborsList ();
		PopulateWallNeighborsList ();
	}

	void PopulateCellNeighborsList() {

		foreach (Cell cell in cells) {
			int check = (((cell.id + 1) / size.x) - 1) * size.x + size.x;

			if (cell.id + 1 < totalCellNum && (cell.id + 1) != check) {
				cells[cell.id].neighbors.Add (cells [cell.id + 1]);

				if (!allWalls [cells [cell.id].north.GetComponent<Wall> ().id].connectingWalls.Contains (allWalls [cells [cell.id + 1].north.GetComponent<Wall> ().id])) {
					allWalls [cells [cell.id].north.GetComponent<Wall> ().id].connectingWalls.Add (allWalls [cells [cell.id + 1].north.GetComponent<Wall> ().id]);
					allWalls [cells [cell.id + 1].north.GetComponent<Wall> ().id].connectingWalls.Add (allWalls [cells [cell.id].north.GetComponent<Wall> ().id]);
				}
			}

			if (cell.id - 1 >= 0 && cell.id != check) {
				cells[cell.id].neighbors.Add (cells [cell.id - 1]);
			}

			if (cell.id + size.x < totalCellNum) {
				cells[cell.id].neighbors.Add (cells [cell.id + size.x]);

				if (!allWalls [cells [cell.id].east.GetComponent<Wall> ().id].connectingWalls.Contains (allWalls [cells [cell.id + 1].east.GetComponent<Wall> ().id])) {
					allWalls [cells [cell.id].east.GetComponent<Wall> ().id].connectingWalls.Add (allWalls [cells [cell.id + size.x].east.GetComponent<Wall> ().id]);
					allWalls [cells [cell.id + size.x].east.GetComponent<Wall> ().id].connectingWalls.Add (allWalls [cells [cell.id].east.GetComponent<Wall> ().id]);
				}
			}

			if (cell.id - size.x >= 0) {
				cells[cell.id].neighbors.Add (cells [cell.id - size.x]);
			}
		}
	}

	void CreateMaze() {
		while (visitedCellNum < totalCellNum) {

			if (startedMazeCreation) {
				GetNeighbors ();

				if (cells [currentNeighbor].visited == false && cells [currentCell].visited == true) {
					//yield return StartCoroutine(BreakWall ());
					//DrawDebugLines(10, cells [currentCell], cells [currentNeighbor]);

					if (movements.ContainsKey (currentCell)) {
						movements [currentCell] = currentNeighbor;
					} else {
						movements.Add (currentCell, currentNeighbor);
						//Debug.Log ("Had key for cell neighbor combo: Current = " + currentCell + " , Neighbor = " + currentNeighbor);
					}

					if (visitedCellNum == 2) {
						boulderSpawnerLocation = (cells[currentCell].transform.position + cells[currentNeighbor].transform.position) / 2.0f;

						Vector3 difference = cells [currentCell].transform.position - cells [currentNeighbor].transform.position;

						if (difference.x != 0) {
							rotateSpawner90Degrees = true;
						}
					}

					BreakWall();
					PutUnicursalWall (currentCell, currentNeighbor);
					cells [currentNeighbor].visited = true;
					visitedCellNum++;
					cellStack.Add (currentCell);
					currentCell = currentNeighbor;

					if (cellStack.Count > 0) {
						backingUp = cellStack.Count - 1;
					}
				}
			} else {
				//currentCell = Random.Range (0, totalCellNum);
				currentCell = 0;
				cells [currentCell].visited = true;

				visitedCellNum++;
				startedMazeCreation = true;
			}

		}

		Debug.Log ("Finished");
	}

	void BreakWall() {
		//Debug.Log("Broke wall #" + wallToBreak);
		Wall wallToDestroy;

		switch (wallToBreak) {
		case 1:
			cells [currentCell].normalWalls.Remove (cells [currentCell].north);
			wallToDestroy = allWalls [cells [currentCell].north.GetComponent<Wall> ().id];

			foreach (Wall connectingWall in wallToDestroy.connectingWalls) {
				connectingWall.connectingWalls.Remove (wallToDestroy);
			}

			cells [currentCell].north.GetComponent<Collider> ().enabled = false;

			Destroy (cells [currentCell].north.gameObject);
			break;
		case 2:
			cells [currentCell].normalWalls.Remove (cells [currentCell].east);
			wallToDestroy = allWalls [cells [currentCell].east.GetComponent<Wall> ().id];

			foreach (Wall connectingWall in wallToDestroy.connectingWalls) {
				connectingWall.connectingWalls.Remove (wallToDestroy);
			}

			cells [currentCell].east.GetComponent<Collider> ().enabled = false;

			Destroy (cells [currentCell].east.gameObject);
			break;
		case 3:
			cells [currentCell].normalWalls.Remove (cells [currentCell].west);
			wallToDestroy = allWalls [cells [currentCell].west.GetComponent<Wall> ().id];

			foreach (Wall connectingWall in wallToDestroy.connectingWalls) {
				connectingWall.connectingWalls.Remove (wallToDestroy);
			}

			cells [currentCell].west.GetComponent<Collider> ().enabled = false;

			Destroy (cells [currentCell].west.gameObject);
			break;
		case 4:
			cells [currentCell].normalWalls.Remove (cells [currentCell].south);
			wallToDestroy = allWalls [cells [currentCell].south.GetComponent<Wall> ().id];

			foreach (Wall connectingWall in wallToDestroy.connectingWalls) {
				connectingWall.connectingWalls.Remove (wallToDestroy);
			}

			cells [currentCell].south.GetComponent<Collider> ().enabled = false;

			Destroy (cells [currentCell].south.gameObject);
			break;
		default:
			break;
		}

		//yield return new WaitForSeconds (1.3f);
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
				//cells[currentCell].neighbors.Add (cells [currentCell + 1]);
				connectingWall [length] = 3;
				length++;
			}
		}

		// east wall
		if (currentCell - 1 >= 0 && currentCell != check) {
			if (cells [currentCell - 1].visited == false) {
				neighbors [length] = currentCell - 1;
				//cells[currentCell].neighbors.Add (cells [currentCell - 1]);
				connectingWall [length] = 2;
				length++;
			}
		}

		// north wall
		if (currentCell + size.x < totalCellNum) {
			if (cells [currentCell + size.x].visited == false) {
				neighbors [length] = currentCell + size.x;
				//cells[currentCell].neighbors.Add (cells [currentCell + size.x]);
				connectingWall [length] = 1;
				length++;
			}
		}

		// south wall
		if (currentCell - size.x >= 0) {
			if (cells [currentCell - size.x].visited == false) {
				neighbors [length] = currentCell - size.x;
				//cells[currentCell].neighbors.Add (cells [currentCell - size.x]);
				connectingWall [length] = 4;
				length++;
			}
		}

		if (length != 0) {
			int randomNeighbor = Random.Range (0, length);
			currentNeighbor = neighbors [randomNeighbor];
			wallToBreak = connectingWall [randomNeighbor];
		} else {
			//DrawDebugLines(10, cells [currentCell], cells [currentNeighbor]);
			if (backingUp > 0) {
				currentCell = cellStack [backingUp];
				//DrawDebugLines(10, cells [currentCell], cells [currentNeighbor]);
				backingUp--;
			}
		}

	}

	void SetCharacter() {
		GameObject player = Instantiate (playerPrefab);
		player.transform.position = cells [0].transform.position;
		player.transform.Translate(new Vector3(-5.0f, 2.0f, -5.0f));

		//dustStorm.signalPlayerAlive (player);
		//dustStorm.setCenterPosition(cells[size.x * size.z / 2].transform.position);
	}

	void FixEntranceWallOrientation() {
		GameObject extraWall = Instantiate (wallPrefabs [0]);
		extraWall.transform.parent = walls.transform;
		extraWall.transform.position = allWalls [0].transform.position;
		extraWall.transform.localScale = allWalls [0].transform.localScale;
		extraWall.transform.Rotate (new Vector3 (0.0f, 90.0f, 0.0f));
	}

	private void DrawDebugLines(int lines, Cell previous, Cell next) {
		for(int i = 0; i < lines; i++) {
			Debug.DrawLine(previous.transform.position, next.transform.position, Color.red, 100.0f);
		}
	}

	private void PutUnicursalWall(int cellNum, int neighborNum) {
		float randomNum;
		randomNum = Random.Range (0.0f, 1.0f);

		GameObject wallPrefabToUse = (randomNum < noTorchWallProbability) ? wallPrefabs [0] : wallPrefabs [1];
		GameObject wallCreated = Instantiate (wallPrefabToUse);
		Wall unicursalWall = wallCreated.GetComponent<Wall> ();

		unicursalWall.transform.parent = walls.transform;
		unicursalWall.transform.position = (cells[cellNum].transform.position + cells[neighborNum].transform.position) / 2.0f;
		unicursalWall.transform.localScale *= wallLength;

		if (Mathf.Abs (cellNum - neighborNum) < 2.0f) {
			unicursalWall.transform.Rotate (new Vector3 (0.0f, 90.0f, 0.0f));
		}

		unicursalWall.cells.Add (cells [cellNum]);
		unicursalWall.cells.Add (cells [neighborNum]);

		cells [cellNum].unicursalWalls.Add (unicursalWall);
		cells [neighborNum].unicursalWalls.Add (unicursalWall);

		unicursalWall.name = "Wall #" + wallNum;
		unicursalWall.isUnicursalAddition = true;
		wallNum++;

		unicursalWalls.Add (unicursalWall);
	}

	void CreateAlcoves() {
		alcovesLeftToBuild = numAlcoves;

		foreach (Cell cell in cells) {
			if (cell.NumSurroundingWalls() == 3 && alcovesLeftToBuild > 0 && cell.id != 0) {
				if (cell.unicursalWalls.Count == 1) {
					Vector3 orientationDirection = (cell.unicursalWalls [0].cells [0].transform.position - cell.unicursalWalls [0].cells [1].transform.position) / 2.0f;
					orientationDirection = orientationDirection.normalized;

					//alcoveTileLocations.Add (cell.unicursalWalls[0].transform.position);
					FindAlcoveTileLocations(cell.unicursalWalls[0].transform.position, orientationDirection);

					destroyedAlcoves.Add(cell.unicursalWalls[0].transform.position + orientationDirection * -15.0f);
					cell.unicursalWalls [0].GetComponent<Collider> ().enabled = false;
					Destroy (cell.unicursalWalls [0].gameObject);
					alcovesLeftToBuild--;
				}
			}

			if (alcovesLeftToBuild > 0 && cell.GetNormalWallsCount() == 2 && cell.unicursalWalls.Count == 2) {
				
				foreach (Cell neighbor in cell.neighbors) {
					if (neighbor.GetNormalWallsCount() == 2 && neighbor.unicursalWalls.Count == 2) {
						if(cell.CommonNormalWallExistsWith (neighbor)) {
							
							if (alcovesLeftToBuild > 0) {
								
								Wall wallToDestroy = cell.GetCommonWall (neighbor);
								if (wallToDestroy != null && wallToDestroy.connectingWalls.Count == 1) {
									Vector3 orientationDirection = wallToDestroy.transform.position - wallToDestroy.connectingWalls [0].transform.position;

									if (wallToDestroy.transform.rotation.y != 0) {
										orientationDirection.z = 0;
									} else {
										orientationDirection.x = 0;
									}

									orientationDirection = orientationDirection.normalized;

									FindAlcoveTileLocations (wallToDestroy.transform.position, -orientationDirection);

									destroyedAlcoves.Add(wallToDestroy.transform.position + orientationDirection * 15.0f);
									//wallToDestroy.connectingWalls[0].connectingWalls.Remove(wallToDestroy);
									wallToDestroy.GetComponent<Collider> ().enabled = false;
									Destroy (wallToDestroy.gameObject);
									alcovesLeftToBuild--;
								}
							}
						}
					}
				}
			}
		}
	}

	void FindAlcoveTileLocations(Vector3 wallToDestroyLocation, Vector3 orientation) {
		Vector3 perpendicular;

		if (orientation.x == 0.0f) {
			perpendicular = new Vector3 (1.0f, 0.0f, 0.0f);
		} else {
			perpendicular = new Vector3 (0.0f, 0.0f, 1.0f);
		}

		alcoveTileLocations.Add (wallToDestroyLocation + orientation * -5.0f + perpendicular * 2.0f + new Vector3(0.0f, 2.0f, 0.0f));
		alcoveTileLocations.Add (wallToDestroyLocation + orientation * -5.0f + perpendicular * -2.0f + new Vector3(0.0f, 2.0f, 0.0f));

		alcoveTileLocations.Add (wallToDestroyLocation + orientation * -15.0f + perpendicular * 2.0f + new Vector3(0.0f, 2.0f, 0.0f));
		alcoveTileLocations.Add (wallToDestroyLocation + orientation * -15.0f + perpendicular * -2.0f + new Vector3(0.0f, 2.0f, 0.0f));
	}

}
