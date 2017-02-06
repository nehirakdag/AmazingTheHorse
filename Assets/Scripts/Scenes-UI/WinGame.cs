using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;
using WindowsInput;

public class WinGame : MonoBehaviour {

	public GameObject winPanel;
	public Button okButton;

	public GameObject character;

	public Texture2D cursorTexture;

	// Use this for initialization
	void Start () {
		okButton.onClick.AddListener (PressEndGame);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Character") {
			winPanel.SetActive (true);
			character.GetComponent<ThirdPersonUserControl> ().enabled = false;
			Cursor.visible = true;
			Cursor.SetCursor (cursorTexture, Vector2.zero, CursorMode.Auto);
			InputSimulator.SimulateKeyPress (VirtualKeyCode.ESCAPE);
		}
	}

	void PressEndGame() {
		Debug.Log ("GAME OVER!");
		Application.Quit ();
	}
}
