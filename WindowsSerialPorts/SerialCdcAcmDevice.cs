using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

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
            List<String> port_list = GetFilteredComList(vid, pid_list);

            if (port_list != null || port_list.Count != 0)
            {
                myComportNumber = port_list[0];
                mySerialPort = new SerialPort();

                mySerialPort.PortName = myComportNumber;
                mySerialPort.BaudRate = 9600;
                mySerialPort.Parity = Parity.None;
                mySerialPort.DataBits = 8;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.Handshake = Handshake.None;
            }



        }


        ~SerialCdcAcmDevice()
        {

        }



        public void StartListening()
        {
            mySerialPort.Open();
            StartListening(serial_recv_wait_handle);
        }

        public void StopListening()
        {
            StopListening(serial_recv_wait_handle);
        }

        private void StartListening(EventWaitHandle waitHandle)
        {
            serial_port_thread_done = false;
            new Thread(() => SerialPortReceive(waitHandle, mySerialPort)).Start();
        }

        private void StopListening(EventWaitHandle waitHandle)
        {
            serial_port_thread_done = true;
            waitHandle?.Set();
        }

        /* Called by the background thread that is polling for received bytes from the serial port. 
         * These are placed into the rx queue. The rx queue is then checked for packets. If any packets
         * are found, the application is notified via the handler. */
        private void SerialPortReceive(EventWaitHandle waitHandle, SerialPort serialPort)
        {
            while (true)
            {


            }




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