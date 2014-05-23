using UnityEngine;
using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class SensationClient {
	private ConcurrentQueue<Sensation> sensationQueue = new ConcurrentQueue<Sensation>();
	
	private Thread transmitThread;
	private EventWaitHandle signal;
	
	private bool shouldStopTransmitting = false;
	private readonly object shouldStopTransmittingLock = new object();

	
	public static readonly SensationClient Instance = new SensationClient();

	// Explicit static constructor to tell C# compiler
	// not to mark type as beforefieldinit
	static SensationClient() {}

	public SensationClient() {
		this.signal = new AutoResetEvent(false);
	}
	
	public void Connect(string serverName) {
		// TODO return if thread already started
		
		transmitThread = new Thread(Transmit);
		transmitThread.Start(serverName);
	}
	
	public void Disconnect() {
		// TODO return if thread not started..
		lock (shouldStopTransmittingLock) {
			shouldStopTransmitting = true;
		}
		signal.Set();
		transmitThread.Join();
		sensationQueue.Clear();
	}
	
	public void SendSensationAsync(Sensation sensation) {
		// TODO Connect if not connected
		sensationQueue.Enqueue(sensation);
		signal.Set();
	}
	
	void Transmit(object serverName) {
		string serverNameString = (string)serverName;
		TcpClient client;
		NetworkStream networkStream;
		
		try {
        	IPAddress[] serverIps = Dns.GetHostEntry(serverNameString).AddressList;
        	if (serverIps.Length < 1) {
				Debug.LogError("Unable to find IP for server " + serverNameString);
				return;
			}
			
			client = new TcpClient();
			client.Connect(new IPEndPoint(serverIps[0], 10000));
			networkStream = client.GetStream();
    	} catch(Exception e) {
			Debug.LogError("Unable to connect to server " + serverNameString);
        	Debug.LogError(e);
			return;
    	}
		
		try {
			while (true) {
				lock (shouldStopTransmittingLock) {
					if (shouldStopTransmitting) {
						break;
					}
				}
				
				if (!client.Connected) {
					Debug.LogError("Unable to send command - client disconnected");
					break;
				}
		
				if (!networkStream.CanWrite) {
					Debug.LogError("Can't write to network stream");
					break;
				}
				
				while (sensationQueue.Count > 0) {
					Sensation sensation;
					bool sensationDequeued = sensationQueue.TryDequeue(out sensation);
					if (!sensationDequeued || sensation == null) {
						break;
					}
					Serializer.SerializeWithLengthPrefix(networkStream, sensation, PrefixStyle.Fixed32BigEndian);
				}
				
				signal.WaitOne();
			}
		} finally {
			networkStream.Close();
			client.Close();
		}
	}
}