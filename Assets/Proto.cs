using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class Proto : MonoBehaviour {
	private TcpClient client;
	private NetworkStream networkStream;
	
	void Awake() {
		this.enabled = InitClient();
	}
	
	private bool InitClient() {
		string serverName = "sensationdriver.local";	// TODO make this a property with default value
		
		try {
            IPAddress[] serverIps = Dns.GetHostEntry(serverName).AddressList;
            if (serverIps.Length < 1) {
				Debug.LogError("Unable to find IP for server " + serverName);
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
		command.ActorIndex = 1;
		command.Intensity = 0.4f;
		command.TargetRegion = Command.Region.LeftForearm;
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
