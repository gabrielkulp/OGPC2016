using UnityEngine;
using System.Collections;

public class CloudGen : MonoBehaviour {
	public GameObject[] cloudPrefab;
	public Vector3 position = Vector3.zero;
	public int seed;// = Mathf.RoundToInt(Random.value * 10000000);
	public int count = 100;
	public AnimationCurve probability;
	public float range = 1000f;
	

	void Start () {
		Random.seed = seed;
		int target = count;
		for (count = 0; count < target; count++) {
			Vector3 pos = Random.insideUnitSphere * range;
			//if (pos.y < 0f)
			//	pos.y *= -1;
			
			if (Random.value < probability.Evaluate(pos.magnitude / range)) {
				/*GameObject newCloud = (GameObject)*/Instantiate(
					cloudPrefab[Mathf.RoundToInt(Random.Range(0, cloudPrefab.Length - 1f))],
					pos + position, Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));
				//newCloud.GetComponent<CS_Cloud>().Scale = Random.Range(1f, 2f);
				//Can't do ^that^ due to nature of cloud system 
			} else
				count -= 1;
		}
	}
	
	void Update () {
		
	}
}
