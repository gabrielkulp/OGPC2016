using UnityEngine;
using System.Collections;

public enum playerMode {
	walking,    //FPS-like, one gameObject
	gliding,    //Airplane flight, one gameObject and one script
	flying      //Blimp flight, one gameObject and one script
}

public class PlayerState : MonoBehaviour {
	public playerMode state = playerMode.walking;
	playerMode prevState = playerMode.walking;
	public GameObject walkingController;
	public AirplaneController airplaneController;
	public GameObject airplaneCam;
	public Docking dockingController;
	public float lastChangeTime = 0f;


	void Start () {
		Cursor.visible = false;
		prevState = state;
		walkingController.SetActive(false);
		airplaneController.enabled = false;
		airplaneCam.SetActive(false);
		dockingController.docked = true;
		ChangeState(prevState, state);
	}
	
	void Update () {
		if (prevState != state)
			ChangeState(prevState, state);

		prevState = state;

		if (Input.GetKeyUp(KeyCode.R))
			Application.LoadLevel(Application.loadedLevel);
	}

	public void ChangeState (playerMode from, playerMode to) {
		lastChangeTime = Time.time;
		state = to;
		prevState = to;
		switch (from) {
			default:
			case playerMode.walking:
				walkingController.SetActive(false);
				break;
			case playerMode.gliding:
				airplaneController.ShutDown();
				airplaneCam.SetActive(false);
				dockingController.docked = true;
				break;
			case playerMode.flying:
				Debug.Log("U WOT M8");
				break;
		}
		switch (to) {
			default:
			case playerMode.walking:
				walkingController.SetActive(true);
				break;
			case playerMode.gliding:
				airplaneController.enabled = true;
				airplaneCam.SetActive(true);
				dockingController.docked = false;
				airplaneCam.GetComponent<AirplaneCam>().ResetPosition();
				break;
			case playerMode.flying:
				Debug.Log("U WOT M8");
				break;
		}
	}
}
