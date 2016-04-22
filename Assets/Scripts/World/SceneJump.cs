using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneJump : MonoBehaviour {

	public int target = 5;

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player")
			SceneManager.LoadScene(target);
	}
}
