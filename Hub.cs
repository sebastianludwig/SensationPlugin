using UnityEngine;
using System;
using System.IO;
using ProtoBuf;

namespace Sensation {

public class Hub : MonoBehaviour {

    [SerializeField]
    private string sensationDriverNetworkName = "sensationdriver.local";

    [SerializeField]
    public bool saveProfilingInformation = false;

    private Profiler _profiler;
    public Profiler profiler {
        get { 
            if (_profiler == null) {
                try {
                    var path = Application.dataPath;
                    if (Application.platform == RuntimePlatform.OSXPlayer) {
                        path += "/../..";
                    } else if (Application.platform == RuntimePlatform.WindowsPlayer) {
                        path += "/..";
                    } else if (Application.isEditor) {
                        path += "/..";
                    }
                    path += "/sensation_profile_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";
                    Debug.Log("Created profiling file at " + path);
                    _profiler = new Profiler(path);
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
            Client.Instance.profiler = saveProfilingInformation ? profiler : null;
        }
    }
    
    void Awake() {
        Client.Instance.AddExceptionDelegate(OnClientException);
        Client.Instance.Connect(sensationDriverNetworkName);
    }

    private void OnClientException(Exception e) {
        Debug.LogException(e);
    }
    
    void OnDestroy() {
        Client.Instance.Disconnect();
        if (_profiler != null) {
            _profiler.Close();
        }
    }

    public void LoadPattern(TextAsset serialized) {
        LoadPattern message = Serializer.Deserialize<LoadPattern>(new MemoryStream(serialized.bytes));

        Client.Instance.SendAsync(message);
    }
    
    public void PlayPattern(string identifier, int priority = 80) {
        PlayPattern message = new PlayPattern();
        message.Identifier = identifier;
        message.Priority = priority;

        Client.Instance.SendAsync(message);
    }
}

}
