using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
	
	public Wall north;
	public Wall east;
	public Wall west;
	public Wall south;
	public GameObject floor;

	public bool visited;

	public int id;

	public List<Cell> neighbors = new List<Cell> ();
	public List<Wall> normalWalls = new List<Wall>();
	public List<Wall> unicursalWalls = new List<Wall>();
	public List<TraversableCell> childCells = new List<TraversableCell>();

	public int NumSurroundingWalls() {
		int numWalls = 4;

		if (north.enabled == false) {
			numWalls--;
		}
		if (east.enabled == false) {
			numWalls--;
		}
		if (west.enabled == false) {
			numWalls--;
		}
		if (south.enabled == false) {
			numWalls--;
		}

		return numWalls;
	}

	public List<Wall> GetNormalWalls() {
		List<Wall> normalWalls = new List<Wall> ();

		if (north.enabled == false) {
			normalWalls.Add (this.north);
		}
		if (east.enabled == false) {
			normalWalls.Add (this.east);
		}
		if (west.enabled == false) {
			normalWalls.Add (this.west);
		}
		if (south.enabled == false) {
			normalWalls.Add (this.south);
		}

		return normalWalls;
	}

	public int GetNormalWallsCount() {
		int normalWallCount = 0;

		foreach (Wall wall in normalWalls) {
			if (wall.enabled == true) {
				normalWallCount++;
			}
		}

		return normalWallCount;
	}

	public List<Wall> GetAllWalls() {
		//List<Wall> allWallsOfCell = this.GetNormalWalls();
		List<Wall> allWallsOfCell = this.normalWalls;

		foreach (Wall unicursalWall in this.unicursalWalls) {
			allWallsOfCell.Add (unicursalWall);
		}

		return allWallsOfCell;
	}

	public bool CommonNormalWallExistsWith(Cell other) {
		List<Wall> thisWalls = this.normalWalls;
		List<Wall> otherWalls = other.normalWalls;

		foreach (Wall wall1 in thisWalls) {
			foreach (Wall wall2 in otherWalls) {
				if (wall1.Equals(wall2)) {
					//Debug.Log (wall1.name + " is common for cells " + this.id + " and " + other.id);
					return true;
				}
			}
		}

		return false;
	}

	public Wall GetCommonWall(Cell other) {
		List<Wall> thisWalls = this.normalWalls;
		List<Wall> otherWalls = other.normalWalls;

		foreach (Wall wall1 in thisWalls) {
			foreach (Wall wall2 in otherWalls) {
				//Debug.Log ("Comparing " + wall1.name + " and " + wall2.name);
				if (wall1.Equals(wall2)) {
					return wall1;
				}
			}
		}

		return null;
	}
}
