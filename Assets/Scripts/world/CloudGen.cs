using UnityEngine;
using System.Collections;

public class CloudGen : MonoBehaviour {
	public GameObject[] cloudPrefab;
	public Vector3 position = Vector3.zero;
	public int seed;
	public int count = 100;
	public AnimationCurve probability;
	public float range = 1000f;
	

	void Start () {
		Random.seed = seed;
		int target = count;
		for (count = 0; count < target; count++) {
			Vector3 pos = Random.insideUnitSphere * range;
			
			if (Random.value < probability.Evaluate(pos.magnitude / range)) {
				GameObject cloud =
				(GameObject)Instantiate(
					cloudPrefab[Mathf.RoundToInt(Random.Range(0, cloudPrefab.Length - 1f))],
					pos + position, Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));
				cloud.transform.parent = transform;
			} else
				count -= 1;
		}
	}
}
