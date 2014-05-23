using UnityEngine;
using System.Collections;

public class SensationProbe : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var sensation = new Sensation();
		sensation.ActorIndex = 0;		
		sensation.TargetRegion = Sensation.Region.LeftForearm;
		
		RaycastHit hitInfo;
		float maxDistance = 10f;
		LayerMask layerMask = -1;
		if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, maxDistance, layerMask)) {
			sensation.Intensity = hitInfo.distance / maxDistance;
			Debug.Log("hit something " + sensation.Intensity);
		} else {
			sensation.Intensity = 0;
			Debug.Log("didn't hit anything");
		}
		
		SensationClient.Instance.SendSensationAsync(sensation);
	}
}
