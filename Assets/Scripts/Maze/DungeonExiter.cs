using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using WindowsInput;

public class DungeonExiter : MonoBehaviour {

	public GameObject displayPanel;
	public GameObject character;

	public Texture2D cursorTexture;

	public Button yesButton;
	public Button noButton;

	public void InitializeButtons() {
		yesButton.onClick.AddListener (LoadMazeScene);
		noButton.onClick.AddListener (CancelPanel);
	}

	void OnTriggerEnter(Collider other) {
		displayPanel.gameObject.SetActive (true);
		//userController.enabled = false;
		character.GetComponent<RigidbodyFirstPersonController>().enabled = false;
		Cursor.visible = true;
		Cursor.SetCursor (cursorTexture, Vector2.zero, CursorMode.Auto);
		InputSimulator.SimulateKeyPress (VirtualKeyCode.ESCAPE);
	}

	void LoadMazeScene() {
		Debug.Log ("LOOL YES");
		SceneManager.LoadScene ("MainScene", LoadSceneMode.Single);
	}

	void CancelPanel() {
		Debug.Log ("LOOL NO");
		//userController.enabled = true;
		character.GetComponent<RigidbodyFirstPersonController>().enabled = true;
		Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
		Cursor.visible = false;
		displayPanel.SetActive (false);
	}
}
