using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	private Rigidbody rb;
	public float speed;

	void Start () {
		rb = this.GetComponent<Rigidbody> ();
		rb.velocity = transform.forward * speed;
		if (rb.velocity.y != 0.0f) {
			rb.velocity = new Vector3 (rb.velocity.x, 0.0f, rb.velocity.z);
		}
	}
}
