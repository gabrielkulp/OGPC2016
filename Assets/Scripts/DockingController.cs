using UnityEngine;
using System.Collections;

public class DockingController : MonoBehaviour {
	GameObject airplane;
	public GameObject playerDock;
	public GameObject player;
	PlayerState playerState;
	public Vector3 undockForce;
	Collider trigger;
	public bool canLaunch = true;

	// Use this for initialization
	void Start () {
		airplane = GameObject.Find("Airplane");
		playerState = GameObject.Find("Player Controller").GetComponent<PlayerState>();
		trigger = GetComponent<Collider>();
	}
	
	void FixedUpdate () {
		if (playerState.changed) {
			airplane.GetComponent<Rigidbody>().isKinematic = !playerState.flying;
			if (playerState.flying)
				airplane.GetComponent<Rigidbody>().AddRelativeForce(undockForce);
        }
		trigger.enabled = playerState.flying;

		//Keeps the airplane or player on the deck
		if (playerState.flying) {
			player.transform.position = playerDock.transform.position;
		} else {
			airplane.transform.position = transform.position;
			airplane.transform.rotation = transform.rotation;
		}
	}

	void OnTriggerEnter (Collider other) {
		if (Time.time - playerState.lastChangeTime < 1f)
			return;
		
		//Get the top level gameobject for the camparison below		
		GameObject otherGO = other.gameObject;
		while (otherGO.transform.parent != null) {
			otherGO = otherGO.transform.parent.gameObject;
		}

		if (!airplane.Equals(otherGO))
			return;

		Dock();
	}

	public void Dock () {
		//Change to walking
		playerState.SetFlyingState(false);
	}
}
