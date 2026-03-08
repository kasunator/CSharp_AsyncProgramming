using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace AwiatOnPeriodicTimerAndEventWaitHandle
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

        private CancellationTokenSource _cts;

        void DoWork()
        {
            Console.WriteLine($"DoWork @ {DateTime.Now}");
        }
        /* What chatGPT suggested was PeriodicTimer.
         * Ex: var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
         * But that is not avaialble in .NET framwork 4.8. So
         we are going to use Task Loop with Task.Delay*/
        public async Task RunAsync(CancellationToken token)
        {

            while (token.IsCancellationRequested == false)
            {
                DoWork();
                await Task.Delay(1000);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this._cts = new CancellationTokenSource();
            
            Task.Run(() => RunAsync(_cts.Token));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }
    }
}
