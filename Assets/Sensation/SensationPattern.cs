using UnityEngine;
using System.Collections;

public class SensationPattern : MonoBehaviour {
	public AnimationCurve demoCurve;
	public float tangentLength = 1;
	public float leftHand_thumb;
	public float leftHand_index;

	void OnDrawGizmos() {
		float scale = 10f;

		Gizmos.color = Color.green;

		Vector3 start = Vector3.zero;
		for (float t = 0; t <= 1f; t += 0.01f) {
			float value = demoCurve.Evaluate(t);
			Vector3 end = new Vector3(t * scale, 0, value * scale);
			Gizmos.DrawLine(start, end);
			start = end;
		}


		Gizmos.color = Color.blue;
		for (int i = 0; i < this.demoCurve.keys.Length - 1; ++i) {
			Keyframe startKey = demoCurve.keys[i];
			Keyframe endKey = demoCurve.keys[i + 1];

			start = new Vector3(startKey.time * scale, 0, startKey.value * scale);
			for (float t = 0; t <= 1f; t += 0.01f) {
				float value = Evaluate(t, startKey, endKey);
				Vector3 end = new Vector3((startKey.time + (endKey.time - startKey.time) * t) * scale, 0, value * scale);
				Gizmos.DrawLine(start, end);
				start = end;
			}
		}

		Gizmos.color = Color.red;
		for (int i = 0; i < this.demoCurve.keys.Length - 1; ++i) {
			Keyframe startKey = demoCurve.keys[i];
			Keyframe endKey = demoCurve.keys[i + 1];
			
			start = new Vector3(startKey.time * scale, 0, startKey.value * scale);
			Vector3 p1 = start;
			Vector3 p2 = new Vector3(endKey.time * scale, 0, endKey.value * scale);
			for (float t = 0; t <= 1f; t += 0.01f) {
				float tangLengthX = Mathf.Abs(p1.x - p2.x) * tangentLength;
				float tangLengthY = tangLengthX;

				Vector3 c1 = p1;
				Vector3 c2 = p2;
				c1.x += tangLengthX;
				c1.z += tangLengthY * startKey.outTangent; 
				c2.x -= tangLengthX; 
				c2.z -= tangLengthY * endKey.inTangent;

				Vector3 value = CalculateBezierPoint(t, p1, c1, c2, p2);
				Vector3 end = value; //new Vector3((startKey.time + (endKey.time - startKey.time) * t) * scale, 0, value * scale);
				Gizmos.DrawLine(start, end);
				start = end;
			}
		}
	}

	Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float u = 1f - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;
		
		Vector3 p = uuu * p0; //first term
		p += 3 * uu * t * p1; //second term
		p += 3 * u * tt * p2; //third term
		p += ttt * p3; //fourth term
		
		return p;
	}

	float Evaluate(float t, Keyframe keyframe0, Keyframe keyframe1)
	{
		float dt = keyframe1.time - keyframe0.time;
		
		float m0 = keyframe0.outTangent * dt;
		float m1 = keyframe1.inTangent * dt;
		
		float t2 = t * t;
		float t3 = t2 * t;
		
		float a = 2 * t3 - 3 * t2 + 1;
		float b = t3 - 2 * t2 + t;
		float c = t3 - t2;
		float d = -2 * t3 + 3 * t2;
		
		return a * keyframe0.value + b * m0 + c * m1 + d * keyframe1.value;
	}
}
