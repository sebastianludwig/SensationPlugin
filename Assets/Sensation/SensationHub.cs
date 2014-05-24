using UnityEngine;
using System;

public class SensationHub : MonoBehaviour {	
	[SerializeField]
	string sensationDriverNetworkName = "sensationdriver.local";
	
    void Awake() {
		SensationClient.Instance.Connect(sensationDriverNetworkName);
    }
	
	void OnDestroy() {
		SensationClient.Instance.Disconnect();
	}
}
