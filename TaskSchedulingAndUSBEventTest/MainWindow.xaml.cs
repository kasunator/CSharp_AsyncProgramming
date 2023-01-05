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

namespace TaskSchedulingTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public delegate void UsbDeviceChange(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
    public partial class MainWindow : Window
    {
        static private event UsbDeviceChange UsbDeviceEvent;

        private delegate void TaskCallBack(string s);
        private event TaskCallBack taskCallBack;

        public MainWindow()
        {
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
                // Check if a device has been changed
                if ((int)wParam == DBT_DEVNODES_CHANGED)
                {
                    Console.WriteLine("Main - Device Changed | " + hwnd + " | " + msg + " | " + wParam + " | " + lParam);
                    UsbDeviceEvent?.Invoke(hwnd, msg, wParam, lParam, ref handled);
                }
            }
            return IntPtr.Zero;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("button click");
            taskCallBack += print_message;
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
            Console.WriteLine(s);
            System.Threading.Thread.Sleep(2000);
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
