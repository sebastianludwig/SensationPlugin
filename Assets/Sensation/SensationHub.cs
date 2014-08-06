using UnityEngine;
using System;
using System.IO;
using ProtoBuf;

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

	public void LoadPattern(TextAsset serialized) {
		LoadPattern message = Serializer.Deserialize<LoadPattern>(new MemoryStream(serialized.bytes));

		SensationClient.Instance.SendAsync(message);
	}
	
	public void PlayPattern(string identifier, int priority = 80) {
		PlayPattern message = new PlayPattern();
		message.Identifier = identifier;
		message.Priority = priority;

		SensationClient.Instance.SendAsync(message);
	}
}
