using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class Sensation : MonoBehaviour {
	private TcpClient client;
	private NetworkStream networkStream;
	
	[SerializeField] 
	string ServerName = "sensationdriver.local";
	
	void Awake() {
		this.enabled = InitClient();
	}
	
	private bool InitClient() {		
		try {
            IPAddress[] serverIps = Dns.GetHostEntry(ServerName).AddressList;
            if (serverIps.Length < 1) {
				Debug.LogError("Unable to find IP for server " + ServerName);
				return false;
			}
			
			client = new TcpClient();
			client.Connect(new IPEndPoint(serverIps[0], 10000));
			networkStream = client.GetStream();
        } catch(Exception e) {
            Debug.LogError(e);
			return false;
        }
		return true;
	}

	void OnDestroy() {
		networkStream.Close();
		client.Close();
	}
	
	void Start() {
	}
	
	void Update () {
		var command = new Command();
		command.ActorIndex = 0;		
		command.TargetRegion = Command.Region.LeftForearm;
		
		RaycastHit hitInfo;
		float maxDistance = 10f;
		LayerMask layerMask = -1;
		if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, maxDistance, layerMask)) {
			command.Intensity = hitInfo.distance / maxDistance;
			Debug.Log("hit something " + command.Intensity);
		} else {
			command.Intensity = 0;
			Debug.Log("didn't hit anything");
		}
		SendViaSocket(command);
	}
	
	void SendViaSocket(Command command) {
		if (!client.Connected) {
			Debug.LogError("Unable to send command - client disconnected");
			return;
		}
		
		if (!networkStream.CanWrite) {
			Debug.LogError("Can't write to network stream");
			return;
		}
		
		Serializer.SerializeWithLengthPrefix(networkStream, command, PrefixStyle.Fixed32BigEndian);
	}
}
