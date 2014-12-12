using UnityEngine;
using System;
using System.Collections;


// Note: Random Cone rays: var coneRandomRotation = Quaternion.Euler (Random.Range (-coneAngle, coneAngle), Random.Range (-coneAngle, coneAngle), 0);

public class SensationProbe : MonoBehaviour {
	public enum OutOfReachValue { Off, EvaluateZero, EvaluateOne }
	public enum UpdateMode { Continuous, OnChange }
	
	static Keyframe[] initialIntensityKeyframes;
	static SensationProbe() {
		initialIntensityKeyframes = new Keyframe[2];
		initialIntensityKeyframes[0] = new Keyframe(0, 1);
		initialIntensityKeyframes[0].outTangent = Mathf.Tan(Mathf.Deg2Rad * -45f);
		initialIntensityKeyframes[1] = new Keyframe(1, 0);
		initialIntensityKeyframes[1].inTangent = Mathf.Tan(Mathf.Deg2Rad * -45f);
	}
	
	[SerializeField]
	Vibration.Region region = Vibration.Region.Chest;
	
	[SerializeField]
	int actorIndex = 0;

    [SerializeField]
    public Vector3 origin = Vector3.zero;
	
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
    private float smoothingFactor = 0.8f;
	
    [SerializeField]
    private float sensitivityInPercent = 1;

	[SerializeField]
	UpdateMode updateMode = UpdateMode.OnChange;
	
    // TODO: Add sampling frequency

    private float averagedIntensity = 0;
	private float lastTransmittedIntensity = float.NaN;
	private SensationHub hub;
	
	void Awake() {
		hub = GameObject.FindObjectOfType(typeof(SensationHub)) as SensationHub;
	}

	void Update() {
		float newIntensity = float.NaN;
		
        Vector3 worldOrigin = transform.TransformPoint(origin);
        Vector3 worldDirection = transform.localToWorldMatrix * (direction.normalized * reach);
        float worldReach = worldDirection.magnitude;

		RaycastHit hitInfo;
		if (Physics.Raycast(worldOrigin, worldDirection.normalized, out hitInfo, worldReach, layerMask)) {
			float ratio = hitInfo.distance / worldReach;
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
			Debug.Log("Sensation probe for " + region + " actor index " + actorIndex + " could not evaluate intensity.");
			return;
		}

        // calculate exponentially decaying moving average
        smoothingFactor = Mathf.Clamp01(smoothingFactor);
        averagedIntensity = smoothingFactor * newIntensity + (1 - smoothingFactor) * averagedIntensity;

        Debug.Log(newIntensity + " - " + averagedIntensity);

		if (updateMode == UpdateMode.OnChange && Mathf.Abs(averagedIntensity - lastTransmittedIntensity) < sensitivityInPercent / 100f) {
			return;
		}

		if (hub && hub.saveProfilingInformation) {
			hub.profiler.Log("probe", actorIndex.ToString(), averagedIntensity.ToString("G20"));
		}

		var vibration = new Vibration();
		vibration.ActorIndex = actorIndex;
		vibration.TargetRegion = region;
		vibration.Intensity = averagedIntensity;

		SensationClient.Instance.SendAsync(vibration);
		
		lastTransmittedIntensity = averagedIntensity;
	}
}
