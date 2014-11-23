using UnityEngine;
using System;
using System.IO;
using ProtoBuf;

public class SensationHub : MonoBehaviour {

	[SerializeField]
	private string sensationDriverNetworkName = "sensationdriver.local";

	[SerializeField]
	public bool saveProfilingInformation = false;

	private SensationProfiler _profiler;
	public SensationProfiler profiler {
		get { 
			if (_profiler == null) {
				try {
					var path = Application.dataPath;
					if (Application.platform == RuntimePlatform.OSXPlayer) {
						path += "/../..";
					} else if (Application.platform == RuntimePlatform.WindowsPlayer) {
						path += "/..";
					}
					path += "/sensation_profile_" + DateTime.Now.ToString("yyyyddMM_HHmmss") + ".txt";
					Debug.Log("Created profiling file at " + path);
					_profiler = new SensationProfiler(path);
				} catch (Exception e) {
					Debug.LogError("Could not create sensation profiling file - disabling profiling.");
					Debug.LogException(e);
				}
			}
			return _profiler;
		}
	}

	void OnValidate() {
		if (Application.isPlaying) {
			SensationClient.Instance.profiler = saveProfilingInformation ? profiler : null;
		}
	}
	
    void Awake() {
		SensationClient.Instance.AddExceptionDelegate(OnClientException);
		SensationClient.Instance.Connect(sensationDriverNetworkName);
    }

	private void OnClientException(Exception e) {
		Debug.LogException(e);
	}
	
	void OnDestroy() {
		SensationClient.Instance.Disconnect();
		if (_profiler != null) {
			_profiler.Close();
		}
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
