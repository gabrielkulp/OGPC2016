using UnityEngine;
using System.Collections;

public enum playerMode {
	walking,	//FPS-like, one gameObject
	airplane,	//Airplane flight, one gameObject and one script
	ship		//Airship flight, one gameObject and one script
}

public class PlayerState : MonoBehaviour {
  	public bool flying = false;
	public bool prevState = false;
	public bool changed = false;
	public GameObject walkingController;
	public AirplaneController airplaneController;
	public GameObject airplaneCam;
	public DockingController dockingController;
	public float lastChangeTime = 0f;


	void Start () {
		prevState = flying;
		walkingController.SetActive(false);
		airplaneController.enabled = false;
		airplaneCam.SetActive(false);
		SetFlyingState(flying);
	}
	
	void Update () {
		changed = (prevState != flying);
		if (changed) {
			SetFlyingState(flying);
		}

		prevState = flying;

		if (Input.GetKeyUp(KeyCode.R) || Input.GetKeyUp(KeyCode.JoystickButton6))
			Application.LoadLevel(Application.loadedLevel);
	}

	public void SetFlyingState (bool to) {
		lastChangeTime = Time.time;
		flying = to;
		changed = true;
		if (flying) {
			walkingController.SetActive(false);

			airplaneController.enabled = true;
			airplaneCam.SetActive(true);
			airplaneCam.GetComponent<AirplaneCam>().ResetPosition();
		} else {
			walkingController.SetActive(true);

			airplaneController.ShutDown();
			airplaneCam.SetActive(false);
		}
	}
}
