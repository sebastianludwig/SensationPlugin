using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class Proto : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("called");
		var command = new Command();
		command.ActorIndex = 1;
		command.Intensity = 0.3f;
		command.TargetRegion = Command.Region.LeftForearm;
		
		try {
            IPAddress[] serverIps = Dns.GetHostEntry("sensationdriver.local").AddressList;
            if (serverIps.Length > 0) {
				SendViaSocket(command, serverIps[0]);
			}
        } catch(Exception e) {
            Debug.Log(e);
        }
		
		// WriteFile(command);
		
	}
	
	void SendViaSocket(Command command, IPAddress serverIp) {
		Debug.Log("CLIENT: Opening connection...");
		using (TcpClient client = new TcpClient())
		{
			client.Connect(new IPEndPoint(serverIp, 10000));
			using (NetworkStream stream = client.GetStream())
			{
				Debug.Log("CLIENT: Got connection; sending data...");
				Serializer.SerializeWithLengthPrefix(stream, command, PrefixStyle.Fixed32BigEndian);
				
				Console.WriteLine("CLIENT: Closing...");
				stream.Close();
			}
			
			client.Close();
		}
	}
	
	void WriteFile(Command command) {
    	using (var file = File.Create("command.bin"))
		{
        	Serializer.Serialize(file, command);
    	}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
