using UnityEngine;
using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public delegate void SensationClientExceptionDelegate(Exception e);

/**
 * The actual transmission over the network is handled in a background thread. Therefore
 * any exceptions can't be reported directly and will be reported to any registered
 * exception handlers. Be careful, they will NOT be called on the main thread!
 **/
public class SensationClient {
	private ConcurrentQueue<Message> messageQueue = new ConcurrentQueue<Message>();
	
	private Thread transmitThread;
	private EventWaitHandle signal;
	
	private bool shouldStopTransmitting = false;
	private readonly object shouldStopTransmittingLock = new object();
	
	private SensationClientExceptionDelegate exceptionDelegate;
	
	#region Singleton
	// singleton implemenation following http://csharpindepth.com/articles/general/singleton.aspx
	public static readonly SensationClient Instance = new SensationClient();

	static SensationClient() {}

	public SensationClient() {
		this.signal = new AutoResetEvent(false);
	}
	#endregion
	
	/**
	  * Exception delegates are called on all kind of occasions:
	  * ArgumentException
	  * 	Hostname is an invalid IP
	  * ArgumentOutOfRangeException
	  * 	Hostname was longer than 255 characters
	  * InvalidOperationException
	  * 	The client is not connected anymore
	  * 	The network stream is not writable
	  * ObejctDisposedException
	  * 	The client has been closed (actually the underlying TcpClient) 
	  * SocketException
	  * 	An error was encountered when resolving the hsotname
	  * 	An error occured when accessing the socket
	**/
	public void AddExceptionDelegate(SensationClientExceptionDelegate exceptionDelegate) {
		if (this.exceptionDelegate == null) {
			this.exceptionDelegate = exceptionDelegate;
		} else {
			this.exceptionDelegate += exceptionDelegate;
		}
	}
	
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
		messageQueue.Clear();
	}
	
	public void SendAsync(Vibration vibration) {
		lock (shouldStopTransmittingLock) {		// this check prevents sensations filling up the queue after disconnecting, which would keep the transmitting thread stuck in the inner while loop
			if (shouldStopTransmitting) {
				return;
			}
		}

		Message message = new Message();
		message.Type = Message.MessageType.Vibration;
		message.Vibration = vibration;

		messageQueue.Enqueue(message);
		signal.Set();
	}
	
	#region background thread
	private void Transmit(object serverName) {
		string serverNameString = (string)serverName;
		
		try {
        	IPAddress[] serverIps = Dns.GetHostEntry(serverNameString).AddressList;
			
			ProcessingLoop(serverIps[0], 10000);
    	} catch(Exception e) {
			if (exceptionDelegate != null) {
				exceptionDelegate(e);
			}
			return;
    	}
	}
	
	private void ProcessingLoop(IPAddress server, int port) {
		using (TcpClient client = new TcpClient(server.ToString(), port))
		using (NetworkStream networkStream = client.GetStream())
		{
			while (true) {
				lock (shouldStopTransmittingLock) {
					if (shouldStopTransmitting) {
						break;
					}
				}
				
				if (!client.Connected) {
					throw new InvalidOperationException("Unable to send command - client disconnected");
				}
		
				if (!networkStream.CanWrite) {
					throw new InvalidOperationException("Can't write to network stream");
				}
				
				if (messageQueue.Count > 100) {
					Debug.LogWarning("More than 100 sensation messages queued for network transmission: " + messageQueue.Count + " messages");
				}
				while (messageQueue.Count > 0) {
					Message message;
					bool messageDequeued = messageQueue.TryDequeue(out message);
					if (!messageDequeued || message == null) {
						break;
					}
					Serializer.SerializeWithLengthPrefix(networkStream, message, PrefixStyle.Fixed32BigEndian);  // this shouldn't throw anything..
				}

				signal.WaitOne();
			}
		}
	}
	#endregion
}
