using UnityEngine;
using System.Collections;

public class QuestHolder : MonoBehaviour {
	public int id;
	string key;

	void Start () {
		key = "islandProgress" + id;
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			PlayerPrefs.SetInt(key, 2);
		}
	}
}
