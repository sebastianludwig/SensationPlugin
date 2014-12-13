using UnityEngine;
using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

namespace Sensation {

public delegate void ClientExceptionDelegate(Exception e);

/**
 * The actual transmission over the network is handled in a background thread. Therefore
 * any exceptions can't be reported directly and will be reported to any registered
 * exception handlers. Be careful, they will NOT be called on the main thread!
 **/
public class Client {
    private ConcurrentQueue<Message> messageQueue = new ConcurrentQueue<Message>();
    
    private Thread transmitThread;
    private EventWaitHandle signal = new AutoResetEvent(false);
    
    private bool shouldStopTransmitting = false;
    private readonly object shouldStopTransmittingLock = new object();
    
    private ClientExceptionDelegate exceptionDelegate;

    public Profiler profiler;
    
    #region Singleton
    // singleton implemenation following http://csharpindepth.com/articles/general/singleton.aspx
    public static readonly Client Instance = new Client();

    static Client() {}

    private Client() {
    }
    #endregion
    
    /**
      * Exception delegates are called on all kind of occasions:
      * ArgumentException
      *     Hostname is an invalid IP
      * ArgumentOutOfRangeException
      *     Hostname was longer than 255 characters
      * InvalidOperationException
      *     The client is not connected anymore
      *     The network stream is not writable
      * ObejctDisposedException
      *     The client has been closed (actually the underlying TcpClient) 
      * SocketException
      *     An error was encountered when resolving the hsotname
      *     An error occured when accessing the socket
    **/
    public void AddExceptionDelegate(ClientExceptionDelegate exceptionDelegate) {
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
        transmitThread.IsBackground = true;     // don't keep the application alive
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
        transmitThread.Join(2000);              // wait for background thread termination, but not too long
        messageQueue.Clear();
    }

    public void SendAsync(Message message) {
        lock (shouldStopTransmittingLock) {     // this check prevents sensations filling up the queue after disconnecting, which would keep the transmitting thread stuck in the inner while loop
            if (shouldStopTransmitting) {
                return;
            }
        }

        messageQueue.Enqueue(message);
        signal.Set();
    }
    
    public void SendAsync(Vibration vibration) {
        Message message = new Message();
        message.Type = Message.MessageType.Vibration;
        message.Vibration = vibration;

        SendAsync(message);
    }

    public void SendAsync(LoadPattern pattern) {
        Message message = new Message();
        message.Type = Message.MessageType.LoadPattern;
        message.LoadPattern = pattern;
        
        SendAsync(message);
    }

    public void SendAsync(PlayPattern pattern) {
        Message message = new Message();
        message.Type = Message.MessageType.PlayPattern;
        message.PlayPattern = pattern;
        
        SendAsync(message);
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
                    if (profiler != null) {         // this isn't properly synchronized to spare the lock during normal operations
                        profiler.Log("send", message.ToString());
                    }
                    Serializer.SerializeWithLengthPrefix(networkStream, message, PrefixStyle.Fixed32BigEndian);  // this shouldn't throw anything..
                }

                signal.WaitOne();
            }
        }
    }
    #endregion
}

}
