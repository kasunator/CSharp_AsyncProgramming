using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using WindowsSerialPorts;
using static WindowsSerialPorts.SetupApiUSBInfo;

namespace WindowsSerialPorts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        String VID = "1234";
        String[] pid_list = new String[1] { "0001" };
        public MainWindow()
        {
            this.Closing += MainWindow_Closing;
            InitializeComponent();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisposeSerialPort();
        }

        SerialCdcAcmDevice serialCdcAcmDevice;




        public void DisposeSerialPort()
        {
            serialCdcAcmDevice?.StopListening(); ;
        }

        private void Start_Cnct_Click(object sender, RoutedEventArgs e)
        {
            //List<SetupApiUSBDevInfo> GetComportInfoList(string vid, string[] pid_list)
            //string[] pid_list = new string[1] { "001" };
            //List<SetupApiUSBDevInfo>  comlist = GetComportInfoList(VID, pid_list);

            serialCdcAcmDevice = new SerialCdcAcmDevice(VID, pid_list);

            //serialCdcAcmDevice.StartListening();
            serialCdcAcmDevice.StartListeningAsync();

        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            serialCdcAcmDevice.StopListening();
        }

        private void Send_Test_Msg_Click(object sender, RoutedEventArgs e)
        {
            serialCdcAcmDevice.SerialPortSend_TestMsg();
        }
    }
}
