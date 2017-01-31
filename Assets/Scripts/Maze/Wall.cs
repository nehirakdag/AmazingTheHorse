using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	public List<Cell> cells = new List<Cell>();
	public List<Wall> connectingWalls = new List<Wall> ();
	public bool isUnicursalAddition;
	public int id;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int GetDimension(int xSize, int zSize) {
		int dimensionChange = (xSize + 1) * zSize;

		if (id < dimensionChange) {
			return 0;
		} else {
			return 1;
		}
	}
}
