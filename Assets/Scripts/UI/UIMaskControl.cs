using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMaskControl : MonoBehaviour {
	Image mask;
	bool open;
//	float lerpConst = 0.1f;
	

	void Start () {
		mask = GetComponent<Image>();
		Close();
	}
	
	public void Open () {
		mask.color = Color.white;
	}

	public void Close () {
		mask.color = Color.clear;
	}
}
