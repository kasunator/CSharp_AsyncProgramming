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
using System.IO;

namespace FileCRCCalculation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string FileLocation = "c:\\Projects\\PC_Software\\UWFU_GitLab\\ufwu_application\\UniversalFreescaleUpdater\\bin\\" +
                                    "Debug_Dev\\UpdateFiles\\FCIJL18\\1.1.2.1\\FCIJL18_V1.pufx";
        string FileLocation2 = "c:\\Projects\\PC_Software\\UWFU_GitLab\\ufwu_application\\UniversalFreescaleUpdater\\bin\\" +
                                "Debug_Dev\\UpdateFiles\\BBY-RP5-GM31-Firmwar\\5.7.4.1\\fw_A-109_Bootloader_Updater_V5.0.pufx";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            uint crc_calc = calculateCRC(FileLocation2);
            Console.WriteLine("CRC value:" + crc_calc.ToString("X"));
        }

        uint calculateCRC(string fileName)
        {
            CRC32_FileCheck crc32 = new CRC32_FileCheck();
            String crcValue = String.Empty;
            uint localDirSize = 0;

            using (FileStream fs = File.Open(fileName, FileMode.Open))
            foreach (byte b in crc32.ComputeHash(fs))
            {
                crcValue += b.ToString("x2").ToLower();
            }
            localDirSize = Convert.ToUInt32(crcValue, 16);
            return localDirSize;
        }

    }
}
