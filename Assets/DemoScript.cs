using UnityEngine;
using System.Collections;

public class DemoScript : MonoBehaviour {
	public TextAsset what;
	public AnimationCurve curve;

	// Use this for initialization
	void Start () {
		var hub = GameObject.FindWithTag("SensationHub");
        
		hub.GetComponent<SensationHub>().LoadPattern(what);
		hub.GetComponent<SensationHub>().PlayPattern("pattern3", 101);

		Debug.Log(curve[0].time + ", " + curve[0].value + " - " + curve[0].outTangent);
		Debug.Log(curve[1].time + ", " + curve[1].value + " - " + curve[1].inTangent);

		float t = 0f;
		string buffer = "";
		while (t < curve[2].time) {
			t += 0.1f;
			buffer += curve.Evaluate(t) + "\n";
		}
		Debug.Log(buffer);
	}

	
	void OnDrawGizmos() {
		Calculation(curve.keys);
	}
	
	void Calculation(Keyframe[] keys) {
		float scale = 1f;
		
		Gizmos.color = Color.red;
		for (int i = 0; i < keys.Length - 1; ++i) {
			Keyframe startKey = keys[i];
			Keyframe endKey = keys[i + 1];
			
			Vector3 start = new Vector3(startKey.time * scale, 0, startKey.value * scale);
			Vector3 p1 = start;
			Vector3 p2 = new Vector3(endKey.time * scale, 0, endKey.value * scale);
			
			float tangLengthX = Mathf.Abs(p1.x - p2.x) / 3f;
			float tangLengthY = tangLengthX;
			
			Vector3 c1 = p1;
			Vector3 c2 = p2;
			c1.x += tangLengthX;
			c1.z += tangLengthY * startKey.outTangent; 
			c2.x -= tangLengthX; 
			c2.z -= tangLengthY * endKey.inTangent;

//			Debug.Log(p1.x + " - " + p1.z);
//			Debug.Log(c1.x + " - " + c1.z);
//			Debug.Log(c2.x + " - " + c2.z);
//			Debug.Log(p2.x + " - " + p2.z);
			
			for (float t = 0; t <= 1.8f; t += 0.1f) {
				Vector3 end = CalculateBezierPoint(t / 1.8f, p1, c1, c2, p2);
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

		//Debug.Log("t: " + t);
		Vector3 p = uuu * p0; //first term
		//Debug.Log("#1: " + p.x + " - " + p.z);
		p += 3 * uu * t * p1; //second term
		//Debug.Log("#2: " + p.x + " - " + p.z);
		p += 3 * u * tt * p2; //third term
		//Debug.Log("#3: " + p.x + " - " + p.z);
		p += ttt * p3; //fourth term
		//Debug.Log("#4: " + p.x + " - " + p.z);
		
		return p;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
