using UnityEngine;
using System.Collections;

public class Docking : MonoBehaviour {
	GameObject playerController;
	GameObject airplane;
	public GameObject playerDock;
	public GameObject player;
	Collider trigger;
	public bool docked = true;
	bool lastDocked = true;

	// Use this for initialization
	void Start () {
		playerController = GameObject.Find("Player Controller");
		airplane = GameObject.Find("Airplane");
		trigger = GetComponent<Collider>();
		docked = true;
	}
	
	void FixedUpdate () {
		if (lastDocked != docked) {
			airplane.GetComponent<Rigidbody>().isKinematic = docked;
		}
		trigger.enabled = !docked;

		//Keeps the airplane on the deck
		if (docked) {
			airplane.transform.position = transform.position;
			airplane.transform.rotation = transform.rotation;
		} else {
			player.transform.position = playerDock.transform.position;
		}
		lastDocked = docked;
	}

	void OnTriggerEnter (Collider other) {
		if (docked || lastDocked || 
			Time.time - GameObject.Find("Player Controller").GetComponent<PlayerState>().lastChangeTime < 1f)
			return;
		
		//Get the top level gameobject for the camparison below		
		GameObject otherGO = other.gameObject;
		while(otherGO.transform.parent != null) {
			otherGO = otherGO.transform.parent.gameObject;
		}

		if (!airplane.Equals(otherGO))
			return;
		
		//Change to walking
		playerController.GetComponent<PlayerState>().state = playerMode.walking;
	}
}
