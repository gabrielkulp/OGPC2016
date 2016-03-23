using UnityEngine;
using System.Collections;

public class FlameFlicker : MonoBehaviour {

	public float divergence = 0.1f;
	public float lerpCoeff = 0.1f;
	float startIntensity;
	Light pointLight;

	void Start () {
		pointLight = GetComponent<Light>();
		startIntensity = pointLight.intensity;
	}
	
	void Update () {
		pointLight.intensity = Mathf.Lerp(pointLight.intensity,
			startIntensity * (1 + Random.Range(-divergence, divergence)),
			lerpCoeff);
	}
}
