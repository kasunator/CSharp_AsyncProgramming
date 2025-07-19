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
        public MainWindow()
        {
            InitializeComponent();
        }

        SerialCdcAcmDevice serialCdcAcmDevice;

        private void Start_Cnct_Click(object sender, RoutedEventArgs e)
        {
            //List<SetupApiUSBDevInfo> GetComportInfoList(string vid, string[] pid_list)
            string[] pid_list = new string[1] { "001" };
            //List<SetupApiUSBDevInfo>  comlist = GetComportInfoList("123", pid_list);

            serialCdcAcmDevice = new SerialCdcAcmDevice("123", pid_list);



        }
    }
}
