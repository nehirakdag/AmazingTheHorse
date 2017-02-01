using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeDirections {

	public static Vector3[] directions = {
		new Vector3 (-1.0f, 0.0f, 0.0f),
		new Vector3 (1.0f, 0.0f, 0.0f),
		new Vector3 ( 0.0f, 0.0f, -1.0f),
		new Vector3 ( 0.0f, 0.0f, 1.0f)
	};

	public static string GetDirectionName(Vector3 direction) {
		if (direction.Equals (new Vector3 (-1.0f, 0.0f, 0.0f))) {
			return "Left";
		}

		if (direction.Equals (new Vector3 (1.0f, 0.0f, 0.0f))) {
			return "Right";
		}

		if (direction.Equals (new Vector3 (0.0f, 0.0f, -1.0f))) {
			return "Down";
		}

		if (direction.Equals (new Vector3 (0.0f, 0.0f, 1.0f))) {
			return "Up";
		}

		return "";
	}

	public static Vector3 GetDirectionBetween(TraversableCell c1, TraversableCell c2) {
		return c2.transform.position - c1.transform.position;
	}
}
