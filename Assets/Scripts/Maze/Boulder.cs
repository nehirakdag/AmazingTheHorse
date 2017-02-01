using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour {

	public float velocity = 20.0f;

	public TraversableCell currentLocationCell;
	public TraversableCell endCell;

	public MazeManager manager;

	private Rigidbody rb;

	public Vector3 currentMovement;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (currentMovement.x != 0) {
			this.transform.Rotate (new Vector3 (0.0f, 0.0f, 10.0f));
		}
		else if(currentMovement.z != 0) {
			this.transform.Rotate (new Vector3 (10.0f, 0.0f, 0.0f));
		}
	}

	public bool RollTowards(Vector3 pointToRoll) {
		Vector3 directionToRoll = (pointToRoll - this.transform.position);
		directionToRoll.y = 0;

		if (directionToRoll.magnitude < 3.0f) {
			return true;
		} else {
			directionToRoll = directionToRoll.normalized;

			rb.AddRelativeForce(directionToRoll * velocity);

			return false;
		}
	}

	public void MoveTowards(Vector3 targetPosition) {
		float step = velocity * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

		currentMovement = (targetPosition - transform.position).normalized;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			Destroy (other.gameObject);
			manager.LaunchReloadPanel ();
		}
	}
}
