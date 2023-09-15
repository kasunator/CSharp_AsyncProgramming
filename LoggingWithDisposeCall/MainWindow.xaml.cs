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

namespace DisposeCall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Logging log;
        int count = 0;
        public MainWindow()
        {
            InitializeComponent();

            // Logging(string tag = "", LogLevel level = LogLevel.DEBUG, bool save2File = false)
            log = new Logging("TEST", Logging.LogLevel.DEBUG,  true);

        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            log.log("count" + count++);
        }

        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            log.requestCancel();
        }
    }
}
