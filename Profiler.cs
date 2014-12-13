using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Sensation {

public class Profiler {
    private static readonly long epochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

    private ConcurrentQueue<string> entries = new ConcurrentQueue<string>();
    
    private Thread writingThread;
    private EventWaitHandle signal = new AutoResetEvent(false);

    private bool shouldStopWriting = false;
    private readonly object shouldStopWritingLock = new object();

    private string logPath;

    public Profiler(string logPath) {
        this.logPath = logPath;

        lock (shouldStopWritingLock) {
            shouldStopWriting = false;
        }
        writingThread = new Thread(WriteLog);
        writingThread.IsBackground = true;      // don't keep the application alive
        writingThread.Priority = ThreadPriority.Lowest;
        writingThread.Start("Sensation.Profiler background logger");
    }

    public void Log(string action, params string[] text) {
        var nowTicks = DateTime.UtcNow.Ticks;
        var millisecondsSinceEpoch = (nowTicks - epochTicks) / TimeSpan.TicksPerMillisecond;
        entries.Enqueue(String.Format("{0};{1:#};{2}", action, millisecondsSinceEpoch, String.Join(";", text)));
        signal.Set();
    }

    public void Close() {
        lock (shouldStopWritingLock) {
            shouldStopWriting = true;
        }
        signal.Set();
        writingThread.Join(10000);              // wait for background thread termination, but not too long
        entries.Clear();
    }

    public void WriteLog() {
        StreamWriter writer = new StreamWriter(logPath, false, System.Text.Encoding.UTF8, 262144);  // 256 KiB buffer

        while (true) {
            lock (shouldStopWritingLock) {
                if (shouldStopWriting) {
                    break;
                }
            }

            while (entries.Count > 0) {     // write everything there is to the file
                string entry;
                bool entryDequeued = entries.TryDequeue(out entry);
                if (!entryDequeued || entry == null) {
                    break;
                }

                writer.WriteLine(entry);
            }
            
            signal.WaitOne();
        }

        writer.Close();
    }
}

}
