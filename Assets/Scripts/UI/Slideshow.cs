using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Slideshow : MonoBehaviour {

	public TextMesh text;
	public AudioSource music;
	public Material[] mats;
	public float[] times;
	MeshRenderer MR;
	float timer;
	int index;
	bool playing = true;

	float dispTimer;
	bool advance = false;

	void Start () {
		Cursor.visible = false;
		MR = GetComponent<MeshRenderer>();
		timer = 0f;
		index = 0;
		MR.material = mats[0];
		playing = true;
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Debug.Log(dispTimer);
			dispTimer = 0;
			advance = true;
		}
		dispTimer += Time.deltaTime;


		if (Input.GetKeyUp(KeyCode.Escape))
			SceneManager.LoadScene(0);
		if (playing) {
			timer += Time.deltaTime;
			if (timer >= times[index] || advance) {
				advance = false;
				timer = 0;
				index++;
				if (index >= mats.Length) {
					playing = false;
					return;
				}
				MR.material = mats[index];
			}
		} else {
			text.color = Color.Lerp(text.color, Color.white, 0.1f);
			music.volume = Mathf.Lerp(music.volume, -0.1f, 0.05f);
			if (Input.anyKey)
				SceneManager.LoadScene(0);
		}
	}
}
