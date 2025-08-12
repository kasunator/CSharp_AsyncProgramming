using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Web;

namespace WindowsSerialPorts
{
    class SerialCdcAcmDevice
    {
        private SerialPort mySerialPort;

        private String myComportNumber;

        /* Bool to indicate to the receive thread if it should exit */
        private volatile bool serial_port_thread_done;

        /* Event object to wait for serial receive from the FTDI driver */
        EventWaitHandle serial_recv_wait_handle = new EventWaitHandle(false, EventResetMode.AutoReset);


        public SerialCdcAcmDevice(String vid, String[] pid_list)
        {
            try
            {
                List<String> port_list = GetFilteredComList(vid, pid_list);

                if (port_list != null && port_list.Count != 0)
                {
                    myComportNumber = port_list[0];
                    mySerialPort = new SerialPort();

                    mySerialPort.PortName = myComportNumber;
                    mySerialPort.BaudRate = 9600;
                    mySerialPort.Parity = Parity.None;
                    mySerialPort.DataBits = 8;
                    mySerialPort.StopBits = StopBits.One;
                    mySerialPort.Handshake = Handshake.None;
                    //MainWindow.AddUsbEvent(DeviceConnectionEvent);


                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Exxcpetion at SerialCdcAcmDevice: {0}", ex.ToString());            
            }

        }


        ~SerialCdcAcmDevice()
        {

        }



        public void StartListening()
        {
            try
            {
                mySerialPort.Open();
                _startListening(serial_recv_wait_handle);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excpetion at StartListening: {0}", ex.ToString());
            }

        }


        public void StartListeningAsync()
        {
            try
            {
                if (mySerialPort != null)
                {
                    mySerialPort.DataReceived += MySerialPort_DataReceived;
                    mySerialPort.ErrorReceived += ErrorReceivedHandler;
                    mySerialPort.Open();
                }
            }
            catch (Exception ex) { Console.WriteLine("Excpetion at StartListening: {0}", ex.ToString()); }

        }
        /* The event handler that will be called when the data is received .
            The handler runs on a background thread.
         */
        private void MySerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                Console.WriteLine("total_bytes_Receved {0}", sp.BytesToRead);
                /*Reading bybte by byte does not work */
                //byte rxByte = (byte)sp.ReadByte();
                //byte[] rxByteArray = new byte[1] { rxByte };
                //Console.WriteLine("{0}", Encoding.UTF8.GetString(rxByteArray));
                byte[] rxByteArray = new byte[sp.BytesToRead];
                sp.Read(rxByteArray, 0, sp.BytesToRead);
                foreach (byte b in rxByteArray) 
                {
                    Console.WriteLine("{0:x}", b);
                }
                

            } catch (Exception ex) 
            { 
                Console.WriteLine("MySerialPort_DataReceived {0}",ex.ToString()); 
            }
        }


        /* the event handler that will be called when there is an error. 
         * The event handler runs on a backroungd thread */
        private static void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine("Serial Error: " + e.EventType);

            switch (e.EventType)
            {
                case SerialError.Frame:
                    Console.WriteLine("Framing error detected.");
                    break;
                case SerialError.Overrun:
                    Console.WriteLine("Overrun error detected.");
                    break;
                case SerialError.RXOver:
                    Console.WriteLine("Input buffer overflow.");
                    break;
                case SerialError.RXParity:
                    Console.WriteLine("Parity error.");
                    break;
                case SerialError.TXFull:
                    Console.WriteLine("Transmit buffer full.");
                    break;
            }
        }

        public void StopListening()
        {
            _stopListening(serial_recv_wait_handle);
        }

        private void _startListening(EventWaitHandle waitHandle)
        {
            serial_port_thread_done = false;
            new Thread(() => SerialPortReceive(waitHandle, mySerialPort)).Start();
        }

        private void _stopListening(EventWaitHandle waitHandle)
        {
            try
            {
                mySerialPort.Close();

                //serial_port_thread_done = true;
                //waitHandle?.Set();

            }
            catch(Exception ex) 
            {
                Console.WriteLine("Exxcpetion at StartListening: {0}", ex.ToString());
            }
        }

        /* Called by the background thread that is polling for received bytes from the serial port. 
         * These are placed into the rx queue. The rx queue is then checked for packets. If any packets
         * are found, the application is notified via the handler. */
        private void SerialPortReceive(EventWaitHandle waitHandle, SerialPort serialPort)
        {
            try
            {
                while (true)
                {
                    byte rxByte= (byte)serialPort.ReadByte();
                    byte[] rxByteArray = new byte[1] { rxByte };
                    Console.WriteLine("{0}", Encoding.UTF8.GetString(rxByteArray) );

                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine("Excpetion at SerialPortReceive: {0}", ex.ToString());
            }

        }

        public void SerialPortSend_TestMsg()
        {
            try
            {
                byte[] txByteArray = new byte[] { 0x12, 0x00, 0xff, 0x00, 0x03, 0x0A, 0x0B, 0x0C, 0x5B, 0xBD, 0x13 };
                mySerialPort.Write(txByteArray, 0, txByteArray.Length);
            }
            catch (Exception ex) { Console.WriteLine("Excpetion at SerialPortSend_TestMsg {0}", ex.ToString()); }
        }

        public void SerialPortSend_RequestInfoMsg()
        {
            try
            {
                byte[] txByteArray = new byte[] { 0x12, 0x00, 0x01, 0x00, 0x2E, 0xE0, 0x13 };
                mySerialPort.Write(txByteArray, 0, txByteArray.Length);
            }
            catch (Exception ex) { Console.WriteLine("Excpetion at SerialPortSend_TestMsg {0}", ex.ToString()); }
        }




        /* Returns a list of comparts that match the passed vid and pid */
        static private List<string> GetFilteredComList(String vid, String[] pid_list)
        {
            List<string> ret_list = new List<string>();
            List<SetupApiUSBInfo.SetupApiUSBDevInfo> setupApiUSBDevInfoList = SetupApiUSBInfo.GetComportInfoList(vid, pid_list);

            if (setupApiUSBDevInfoList.Count > 0)
            {
                foreach (SetupApiUSBInfo.SetupApiUSBDevInfo info in setupApiUSBDevInfoList)
                {
                    ret_list.Add(info.port_name);
                }
            }

            return ret_list;

        }



    }
}