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
	
	#region Singleton
	// singleton implemenation following http://csharpindepth.com/articles/general/singleton.aspx
	public static readonly SensationClient Instance = new SensationClient();

	static SensationClient() {}

	public SensationClient() {
		this.signal = new AutoResetEvent(false);
	}
	#endregion
	
	public void Connect(string serverName) {
		if (transmitThread != null && transmitThread.IsAlive) {
			Debug.Log("Sensation client already connected - disconnect before reconnecting");
			return;
		}
		
		lock (shouldStopTransmittingLock) {
			shouldStopTransmitting = false;
		}
		transmitThread = new Thread(Transmit);
		transmitThread.IsBackground = true;		// don't keep the application alive
		transmitThread.Start(serverName);
	}
	
	public void Disconnect() {
		if (transmitThread == null || !transmitThread.IsAlive) {
			return;
		}
		lock (shouldStopTransmittingLock) {
			shouldStopTransmitting = true;
		}
		signal.Set();
		transmitThread.Join(2000);				// wait for background thread termination, but not too long
		sensationQueue.Clear();
	}
	
	public void SendSensationAsync(Sensation sensation) {
		lock (shouldStopTransmittingLock) {		// this check prevents sensations filling up the queue after disconnecting, which would keep the transmitting thread stuck in the inner while loop
			if (shouldStopTransmitting) {
				return;
			}
		}
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
			
			ProcessingLoop(serverIps[0], 10000);
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
	
	private void ProcessingLoop(IPAddress server, int port) {
		using (TcpClient client = new TcpClient(server.ToString(), port))
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