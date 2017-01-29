using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;
using WindowsInput;

public class DungeonLoader : MonoBehaviour {

	public GameObject displayPanel;
	public GameObject character;

	public Texture2D cursorTexture;

	//public ThirdPersonCharacter character;
	private ThirdPersonUserControl userControl;

	public Button yesButton;
	public Button noButton;

	// Use this for initialization
	void Start () {
		yesButton.onClick.AddListener (LoadMazeScene);
		noButton.onClick.AddListener (CancelPanel);

		character = GameObject.FindGameObjectWithTag ("Character");

		userControl = character.GetComponent<ThirdPersonUserControl> ();
	}

	void OnTriggerEnter(Collider other) {
		displayPanel.gameObject.SetActive (true);
		userControl.enabled = false;
		Cursor.visible = true;
		Cursor.SetCursor (cursorTexture, Vector2.zero, CursorMode.Auto);
		InputSimulator.SimulateKeyPress (VirtualKeyCode.ESCAPE);
	}

	void LoadMazeScene() {
		Debug.Log ("LOOL YES");
		SceneManager.LoadScene ("MazeScene", LoadSceneMode.Single);
	}

	void CancelPanel() {
		Debug.Log ("LOOL NO");
		userControl.enabled = true;
		Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
		Cursor.visible = false;
		displayPanel.SetActive (false);
	}
}
