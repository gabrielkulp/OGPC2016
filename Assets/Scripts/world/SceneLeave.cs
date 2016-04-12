using UnityEngine;
using System.Collections;

public class SceneLeave : MonoBehaviour {
	public CrossSceneController sceneController;
	public int targetScene = 2;
	public Transform respawnPos;

	void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
			other.GetComponent<AudioSource>().Stop();
			sceneController.LoadSideLevel(targetScene, respawnPos.position);
		}
	}
}
