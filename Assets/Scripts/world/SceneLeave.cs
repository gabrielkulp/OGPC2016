﻿using UnityEngine;
using System.Collections;

public class SceneLeave : MonoBehaviour {
	public CrossSceneController sceneController;
	public int targetScene = 2;
	public Transform respawnPos;

	public Transform player;

	void OnTriggerEnter (Collider other) {
		if (other.transform.Equals(player)) {
			sceneController.LoadSideLevel(targetScene, respawnPos.position);
		}
	}
}
