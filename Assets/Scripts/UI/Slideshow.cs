using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Slideshow : MonoBehaviour {

	public Material[] mats;
	public float[] times;
	MeshRenderer MR;
	float timer;
	int index;

	void Start () {
		MR = GetComponent<MeshRenderer>();
		timer = 0f;
		index = 0;
		MR.material = mats[0];
	}
	
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape))
			SceneManager.LoadScene(0);
		timer += Time.deltaTime;
		if (timer >= times[index]) {
			if (index >= mats.Length) {
				End();
				return;
			}
			index++;
			timer = 0;
			MR.material = mats[index];
		}
	}

	void End() {

	}
}
