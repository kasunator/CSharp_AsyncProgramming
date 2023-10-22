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

namespace TaksWithinTasks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /* The purpose of this project is to demonstate using the async await synchronous programming practicess  when task within a task is created. I specillay had problems when understatnding the 
         * return types in such scenarios. 
         * 
         * I had to encounter this senario when I had to call sql.sqlCommandsAsync() an another task and await for it in the UI task. I will create a hypothetical function called
         * asynchronousIO, which will be similiar to the sql.sqlCommandsAsync()*/

        public MainWindow()
        {
            InitializeComponent();
        }

        private Task<int> asynchronousIO(int i)
        {
            return Task.Factory.StartNew(() => { Thread.Sleep(1000 * i); return i; }, TaskCreationOptions.LongRunning);
        }

        private async void StartIOAndWaitInBackgorundThread()
        {
            Task<Task<int>> task = Task.Factory.StartNew(async () => { int result = await asynchronousIO(5); Thread.Sleep(5000);  return result; });
            Task<int> inner_task = await task;
            Console.WriteLine("outer task completed");
            int inner_task_result = await inner_task;
            Console.WriteLine("inner task completed:{0}", inner_task_result);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartIOAndWaitInBackgorundThread();
            Console.WriteLine("AsycIOTasksStarted");
        }
    }
}
