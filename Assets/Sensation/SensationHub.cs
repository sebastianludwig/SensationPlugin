using UnityEngine;
using System;

public class SensationHub : MonoBehaviour {	
	[SerializeField]
	string sensationDriverNetworkName = "sensationdriver.local";
	
    void Awake() {
		SensationClient.Instance.AddExceptionDelegate(OnClientException);
		SensationClient.Instance.Connect(sensationDriverNetworkName);
    }
	
	private void OnClientException(Exception e) {
		Debug.LogException(e);
	}
	
	void OnDestroy() {
		SensationClient.Instance.Disconnect();
	}
}
