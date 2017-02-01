using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversableCell : MonoBehaviour {

	public int id;
	public Cell parent;
	public List<TraversableCell> connectedNeighbors;

	public bool visited = false;
	public bool navigatable = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
