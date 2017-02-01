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

	public AudioSource audioSource;

	public bool isActivated = false;

	void Start() {
		audioSource = GetComponent<AudioSource> ();
	}

	public void InitializeButtons() {
		yesButton.onClick.AddListener (LoadMazeScene);
		noButton.onClick.AddListener (CancelPanel);
	}

	void OnTriggerEnter(Collider other) {
		if (isActivated && other.gameObject.tag == "Player") {
			displayPanel.gameObject.SetActive (true);
			//userController.enabled = false;
			audioSource.Play();
			character.GetComponent<RigidbodyFirstPersonController> ().enabled = false;
			//character.gameObject.SetActive (false);
			Cursor.visible = true;
			Cursor.SetCursor (cursorTexture, Vector2.zero, CursorMode.Auto);
			InputSimulator.SimulateKeyPress (VirtualKeyCode.ESCAPE);
		}
	}

	void LoadMazeScene() {
		Debug.Log ("LOOL YES");
		SceneManager.LoadScene ("MainSceneEnd", LoadSceneMode.Single);
	}

	void CancelPanel() {
		Debug.Log ("LOOL NO");
		//userController.enabled = true;
		//character.gameObject.SetActive (true);
		character.GetComponent<RigidbodyFirstPersonController>().enabled = true;
		Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
		Cursor.visible = false;
		displayPanel.SetActive (false);
	}
}
