using UnityEngine;
using System.Collections;

public class SensationProbe : MonoBehaviour {
	public enum OutOfReachValue { Off, EvaluateZero, EvaluateOne }
	public enum UpdateMode { Continues, OnChange }
	
	static Keyframe[] initialIntensityKeyframes;
	static SensationProbe() {
		initialIntensityKeyframes = new Keyframe[2];
		initialIntensityKeyframes[0] = new Keyframe(0, 1);
		initialIntensityKeyframes[0].outTangent = Mathf.Tan(Mathf.Deg2Rad * -45f);
		initialIntensityKeyframes[1] = new Keyframe(1, 0);
		initialIntensityKeyframes[1].inTangent = Mathf.Tan(Mathf.Deg2Rad * -45f);
	}
	
	[SerializeField]
	Sensation.Region region = Sensation.Region.LeftForearm;
	
	[SerializeField]
	int actorIndex = 0;
	
	[SerializeField]
	public Vector3 direction = Vector3.up;
	
	[SerializeField]
	public float reach = 10f;
	
	[SerializeField]
	LayerMask layerMask = -1;
	
	[SerializeField]
	AnimationCurve intensity = new AnimationCurve(initialIntensityKeyframes);
	
	[SerializeField]
	OutOfReachValue outOfReachValue = OutOfReachValue.Off;
	
	[SerializeField]
	UpdateMode updateMode = UpdateMode.OnChange;
	
	float previousIntensity = float.NaN;
	
	void Start() {
		direction.Normalize();
	}
	
	void Update() {
		float newIntensity = float.NaN;
		
		RaycastHit hitInfo;
		if (Physics.Raycast(transform.position, transform.rotation * direction.normalized, out hitInfo, reach, layerMask)) {
			float ratio = hitInfo.distance / reach;
			newIntensity = intensity.Evaluate(ratio);
		} else {
			if (outOfReachValue == OutOfReachValue.Off) {
				newIntensity = 0;
			} else if (outOfReachValue == OutOfReachValue.EvaluateZero) {
				newIntensity = intensity.Evaluate(0f);
			} else if (outOfReachValue == OutOfReachValue.EvaluateOne) {
				newIntensity = intensity.Evaluate(1f);
			}
		}
		
		if (float.IsNaN(newIntensity)) {
			// TODO log warning
			return;
		}
		
		if (updateMode == UpdateMode.OnChange && Mathf.Abs(newIntensity - previousIntensity) < 0.001) {
			Debug.Log("same same");
			return;
		}
		
		Debug.Log("but different");
		var sensation = new Sensation();
		sensation.ActorIndex = actorIndex;
		sensation.TargetRegion = region;
		sensation.Intensity = newIntensity;
		
		SensationClient.Instance.SendSensationAsync(sensation);
		
		previousIntensity = newIntensity;
	}
}
