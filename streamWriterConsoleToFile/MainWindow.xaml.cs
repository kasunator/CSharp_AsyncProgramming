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

namespace streamWriterConsoleToFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// /*
    /// what is the differnece between FileStream and StreamWriter:
    /// 
    /// A FileStram is a subclass of a Stream class,
    /// which deals with byte[].
    /// 
    /// The StreamWriter is a subclass of a TextWriter, 
    /// Which can encode primitives like string , int and char to byte[].
    /// 
    /// You use a bare FileStream when you have to deal with byte[] data.
    /// you add a StreamWriter when you have to write text.
    /// 
    /// StreamWriter is designed for character output in a particular encoding,
    /// whereas classes derived from Stream are designed for byte input and output.
    /// 
    /// Disposal:
    /// Both of these classes implement the IDisposable interface.
    /// When we are done with their object we should dispose them either 
    /// directly or indirectly. 
    /// To dispose directly call its dispose method in a try/catch block.
    /// To dispose indirectly, use the language construct "using" .
    /// 
    /// Creating a StreamWriter:
    /// A stream writer can be created from a filename and a filestream
    /// FileStream stream = new FileStream(fileName, FileMode.CreateNew);  
    /// StreamWriter writer = new StreamWriter(stream)
    /// Or
    /// tring fileName = @"C:\Temp\CSharpAuthors.txt"; 
    /// StreamWriter writer = new StreamWriter(fileName)
    /// 
    /// Additional 
    /// A good practice is to use these objects in a using statement 
    /// so that the unmanaged resources are correctly disposed. 
    /// The using statement automatically calls Dispose on the object 
    /// when the code that is using it has completed.
    /// */
    public partial class MainWindow : Window
    {
        private String FileName = @".\StreamWriter.txt";
        private StreamWriter writer;
        public MainWindow()
        {
           InitializeComponent();
           StreamWriter writer = new StreamWriter(FileName);
        }

        void writeToFileWithStreamWriter(int i)
        {
            try
            {
                using (writer = new StreamWriter(this.FileName, true))
                {
                    Console.WriteLine("starting WriteLine"+ i.ToString());
                    writer.WriteLine("Hello writeLine"+ i.ToString());
                    Console.WriteLine("ending WriteLine" + i.ToString());
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
        /*
         * StreamWriter.WriteLineAsync() method writes a char or char array to a new asynchronously. 
         * The method returns a Task that represents the asynchronous write operation
         */
        void writeToFileWithStreamWriterAsync(int i)
        {
            try
            {
                using (writer = new StreamWriter(this.FileName, true))
                {
                    Console.WriteLine("starting WriteLineAsync" + i.ToString());
                    writer.WriteLineAsync("Hello writeLine" + i.ToString());
                    Console.WriteLine("ending WriteLineAsync" + i.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void StreamWriter_Click(object sender, RoutedEventArgs e)
        {
            writeToFileWithStreamWriter(1);
        }

        private void FileStream_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
