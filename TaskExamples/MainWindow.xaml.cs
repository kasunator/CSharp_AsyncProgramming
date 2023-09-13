using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskExamples
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //start_longRunning_Task();
            start_longRunning_Task_fromFunction();
        }

        void start_longRunning_Task()
        {
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task Started ");
                Thread.Sleep(100);
                Console.WriteLine("Task Ended ");
            }, TaskCreationOptions.LongRunning);
        }

        void long_running_function(int param)
        {
            Console.WriteLine("Task Started from function {0}",param);
            Thread.Sleep(100);
            Console.WriteLine("Task Ended from function {0}", param);
        }


        void start_longRunning_Task_fromFunction()
        {
            Task.Factory.StartNew(() => long_running_function(10), TaskCreationOptions.LongRunning);
        }


    }
}
