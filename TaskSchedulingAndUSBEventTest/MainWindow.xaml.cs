using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Threading;

namespace TaskSchedulingTest
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /*
     * The test:
     * 
     * I want to demonstrate how the WndProc() gets blocked if there is heavy usage of the UI thread.
     * To keep the UI thread bussy I will inititate a background task UpdateTaskUsingDispatch. Which will run the
     * printOnUIThread() function on the UI thread by using theDispatcher.BeginInvoke().
     * The printOnUIThread() has a sleep function delay of which will block the UI thread for longer, So making the
     * sleep delay longer will increase the usage of the UI thread. 
     * The delay in sleep function call inside the UpdateTaskUsingDispatch() will decrease the number of beginInvokes to the UI
     * thread therefore decrase the usage of the UI thread to certain extent.
     * 
     * 
     * Observation:
     * We clearly see that the when the UI thread is uitlized heavyly the number of times WndPRoc() gets called when connecting or disconnecting USb device gets
     * reduced to 1 instead of 2. The observable delay between when we connect the USB device and the actual WndProc()  call also get increased as the UI thread becomes more utilized/bussy.
     * 
     * 
     * 
     * 
     */
    public delegate void UsbDeviceChange(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
    public partial class MainWindow : Window
    {
        static private event UsbDeviceChange UsbDeviceEvent;

        private delegate void TaskCallBack(string s);
        private event TaskCallBack taskCallBack;

        public MainWindow()
        {

            Thread thread = Thread.CurrentThread;
            if (thread.Name == null)
            {
                Console.WriteLine("In MainWindow Thread Name is null");
                thread.Name = "MainWindowTask";
            }
            else
            {
                Console.WriteLine("In MainWindow Thread Name is: {0}", thread.Name);
            }
            InitializeComponent();
            UsbDeviceEvent += UsbDeviceChangeStartTask;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        private const int WM_DEVICECHANGE = 0x0219;               // device change event       
        private const int DBT_DEVNODES_CHANGED = 0x0007;          // A device has been added or removed from the system
        private const int DBT_DEVICEARRIVAL = 0x8000;             // system detected a new device    

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                Thread thread = Thread.CurrentThread;
                if (thread.Name == null)
                {
                    Console.WriteLine("In WndProc Thread Name is null");
                    thread.Name = "WndProcThread";
                }
                else
                {
                    Console.WriteLine("In WndProc Thread Name is: {0}", thread.Name);
                }

                // Check if a device has been changed
                if ((int)wParam == DBT_DEVNODES_CHANGED)
                {
                    Console.WriteLine("Main - Device Changed | " + hwnd + " | " + msg + " | " + wParam + " | " + lParam);
                    UsbDeviceEvent?.Invoke(hwnd, msg, wParam, lParam, ref handled);
                    /*UsbDeviceChangeStartTask(hwnd, msg, wParam, lParam, ref handled);*/


                }
            }
            return IntPtr.Zero;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = Thread.CurrentThread;
            if (thread.Name == null)
            {
                Console.WriteLine("In Button click Thread Name is null");
                thread.Name = "Button_Click_Thread";
            }
            else
            {
                Console.WriteLine("In Button click Thread Name is: {0}", thread.Name);
            }



           // Console.WriteLine("button click");
            //taskCallBack += print_message;
            Task.Factory.StartNew(() => UpdateTaskUsingDispatch());
            //Task.Factory.StartNew(() => UpdateTaskA());
            //Task.Factory.StartNew(() => UpdateTaskB());
            //Task.Factory.StartNew(() => UpdateTaskC());
            //Task.Factory.StartNew(() => UpdateTaskD());
            //Task.Factory.StartNew(() => UpdateTaskE());

        }


        void UsbDeviceChangeStartTask(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            Task.Factory.StartNew(() => UsbTask(hwnd, msg, wParam, lParam, true));
        }


        private void UpdateTaskUsingDispatch()
        {
            for (int i = 0; i < 1000; i++)
            {
                //Console.WriteLine("count: " + i);
               Dispatcher.BeginInvoke(new Action(() =>
               {
                   printOnUIThread("usingDispatch:" + i);

               }));
               System.Threading.Thread.Sleep(100);
            }
        }

        void printOnUIThread(string s)
        {
            Thread thread = Thread.CurrentThread;

            Console.WriteLine("Print on UI Thread Name is: {0} "+s, thread.Name);
            //Console.WriteLine(s);
            System.Threading.Thread.Sleep(5000);
        }

        private void UpdateTaskA()
        {
            for (int i = 0; i < 1000; i++)
            {
                //Console.WriteLine("count: " + i);
                taskCallBack?.Invoke("countA: " + i);
                System.Threading.Thread.Sleep(100);
            }
        }

        private void UpdateTaskB()
        {
            for (int i = 0; i < 1000; i++)
            {
                //Console.WriteLine("count: " + i);
                taskCallBack?.Invoke("countB: " + i);
                System.Threading.Thread.Sleep(100);
            }
        }

        private void UpdateTaskC()
        {
            for (int i = 0; i < 1000; i++)
            {
                //Console.WriteLine("count: " + i);
                taskCallBack?.Invoke("countC: " + i);
                System.Threading.Thread.Sleep(100);
            }
        }

        private void UpdateTaskD()
        {
            for (int i = 0; i < 1000; i++)
            {
                //Console.WriteLine("count: " + i);
                taskCallBack?.Invoke("countD: " + i);
                System.Threading.Thread.Sleep(100);
            }
        }

        private void UpdateTaskE()
        {
            for (int i = 0; i < 1000; i++)
            {
                //Console.WriteLine("count: " + i);
                taskCallBack?.Invoke("countE: " + i);
                System.Threading.Thread.Sleep(100);
            }
        }

        private void UsbTask(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam,  bool handled)
        {
            Console.WriteLine("USB Task:" + hwnd + msg + wParam + lParam + handled);
        }

        void print_message(string s)
        {
            Console.WriteLine(s);
        }
    }
}
