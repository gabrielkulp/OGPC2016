using UnityEngine;
using System.Collections;

public class FloatCam : MonoBehaviour {

	void Start () {
		AssignFloatCam();
    }

	void AssignFloatCam () {
		Player player = GameObject.Find("Player").GetComponent<Player>();
		player.floatCam = this.transform;
	}
}
