using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	public AudioClip shipMusic;
	public AudioClip planeMusic;
	public AudioClip islandMusic;
	public Player player;

	void LateStart () {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	void Update () {
		
	}
}
