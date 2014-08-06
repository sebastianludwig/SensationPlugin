using UnityEngine;
using System.Collections;

public class DemoScript : MonoBehaviour {
	public TextAsset what;

	// Use this for initialization
	void Start () {
		var hub = GameObject.FindWithTag("SensationHub");
		hub.GetComponent<SensationHub>().LoadPattern(what);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
