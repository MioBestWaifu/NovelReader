using Maria.Common.Communication.Commanding;
using Maria.Services.Recordkeeping;
using Maria.Services.Recordkeeping.Records;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Maria.Services.Tracking
{
    internal class ProcessTracker : Tracker
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, IntPtr lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private static IntPtr previousForegroundWindow = IntPtr.Zero;

        private static Thread scannerThread;

        public static ProcessTracker Instance { get { 
                return instance ??= new ProcessTracker();
            } 
            private set { 
                instance = value;
            } }

        private static ProcessTracker instance;

        private ProcessTracker()
        {
            //This is a singleton
        }

        public override async Task<int> Register(Command command)
        {
            //Duplicate code with BrowserTracker
            try
            {
                TrackingRecord record = new TrackingRecord();
                //not always true, may be window, depends on the process
                record.Name = command.Options["name"];
                TimeSpan timestamp = DateTime.Now.TimeOfDay;
                record.Time = timestamp.ToString(@"hh\:mm\:ss");
                await Writer.Instance.AddProcessRecord(record);
                return 200;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 500;
            }

        }

        public override bool Validate(Command command)
        {
            return true;
        }

        public void ScanAndRegister()
        {
            IntPtr foregroundWindow = GetForegroundWindow();

            if (foregroundWindow != previousForegroundWindow)
            {
                previousForegroundWindow = foregroundWindow;

                string title = GetWindowTitle(foregroundWindow).ToLower();
                string processName = GetProcessName(foregroundWindow).ToLower();
                Console.WriteLine($"Foreground Window Title: {title}, Process Name: {processName}");
                Command command = new Command();
                if (processName == "explorer")
                {
                    if (string.IsNullOrEmpty(title))
                    {
                        command.Options.Add("name", "workarea");
                    } else
                    {
                        command.Options.Add("name", title);
                    }
                } else if (processName == "applicationframehost")
                {
                    command.Options.Add("name", title);
                } else
                {
                    command.Options.Add("name", processName);
                }
                Register(command);
            }
        }

        private string GetWindowTitle(IntPtr hWnd)
        {
            const int nChars = 256;
            IntPtr buffer = Marshal.AllocHGlobal(nChars * 2); // 2 bytes per character for Unicode
            GetWindowText(hWnd, buffer, nChars);
            string title = Marshal.PtrToStringUni(buffer);
            Marshal.FreeHGlobal(buffer);
            return title;
        }

        private string GetProcessName(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out uint processId);
            Process process = System.Diagnostics.Process.GetProcessById((int)processId);
            return process.ProcessName;
        }

        public override void Start()
        {
            Console.WriteLine("Process Tracker Started");
            base.Start();
            if (scannerThread == null)
            {
                scannerThread = new Thread(() =>
                {
                    while (Running)
                    {
                        Console.WriteLine("Scanning");
                        ScanAndRegister();
                        Thread.Sleep(1000);
                    }
                });
                scannerThread.Start();
            }
        }
    }
}
