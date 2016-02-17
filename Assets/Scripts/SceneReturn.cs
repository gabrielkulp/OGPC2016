using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneReturn : MonoBehaviour {
	public GameObject player;
	public CrossSceneController sceneController;

	void Start () {
		player = GameObject.Find("Player");
		sceneController = GameObject.Find("SceneController").GetComponent<CrossSceneController>();
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.Equals(player)) {
			sceneController.ReturnFromSideLevel();
		}
	}
}
