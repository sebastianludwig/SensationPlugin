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
	// not to mark type as beforefieldinit (see http://csharpindepth.com/articles/general/singleton.aspx)
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
	
	#region background thread
	private void Transmit(object serverName) {
		string serverNameString = (string)serverName;
		
		try {
        	IPAddress[] serverIps = Dns.GetHostEntry(serverNameString).AddressList;
        	if (serverIps.Length < 1) {
				Debug.LogError("Unable to find IP for server " + serverNameString);
				return;
			}
			
			ProcessingLoop(new IPEndPoint(serverIps[0], 10000));
    	} catch(Exception e) {
			// TODO finer exception handling
			// GetHostEntry
			// whatever ProcessingLoop throws 
			//		TcpClient.Connect() (probably implicitly called)
			//		TcpClient.GetStream()
			//		own exceptions if !connected or !canWrite
			//		Serializer.SerializeWithLengthPrefix()?
			Debug.LogError("Unable to connect to server " + serverNameString);
        	Debug.LogError(e);
			return;
    	}
	}
	
	private void ProcessingLoop(IPEndPoint server) {
		using (TcpClient client = new TcpClient(server))
		using (NetworkStream networkStream = client.GetStream())
		{
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
	#endregion
}