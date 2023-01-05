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
using System.Threading;

namespace AsyncAwaitPitfalls
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


        /* we can of course await on a function that returns task*/
        Task SleepTaskAsyn(int count)
        {
            return Task.Run(() => { Thread.Sleep(count); });
        }
        private int flag = 1;
        async void startSleepAndReturn(int count)
        {
            if (flag == 1)
            {
                await SleepTaskAsyn(count);
                Console.WriteLine("sleep Completed:");
            }
            else
            {
                Console.WriteLine("Flag is not 1");
            }
            Console.WriteLine("Flag check completed");
        }
        /*
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Button Click start");
            startSleepAndReturn(5000);

            Console.WriteLine("Button Click stop");
        }*/

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            this.flag = 1;
            Console.WriteLine("Button 1 Click start");
            startSleepAndReturn(5000);

            Console.WriteLine("Button 1 Click stop");
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            this.flag = 0;
            Console.WriteLine("Button 2 Click start");
            startSleepAndReturn(5000);

            Console.WriteLine("Button 2 Click stop");
        }
    }
}
