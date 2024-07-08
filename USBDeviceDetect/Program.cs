using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace USBDeviceDetect
{
    internal class Program
    {
        static void Main(string[] args)
        {
        }

        public static int GetConnectDevices()
        {


            return 0;
        }

        private static Dictionary<string, ManagementBaseObject> GetPnpEntityList(string[] vidListStr = null)
        {
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity"))
                collection = searcher.Get();

            Dictionary<string, ManagementBaseObject> keyValuePairs = new Dictionary<string, ManagementBaseObject>();

            List<ManagementBaseObject> deviceObjectList = new List<ManagementBaseObject>();

            foreach (var device in collection)
            {
                bool addDevice = false;
                String pnpDeviceID = (string)device.GetPropertyValue("PNPDeviceID");
                if (vidListStr != null)
                {
                    String deviceIdStr = (string)device.GetPropertyValue("DeviceID");
                    foreach (String s in vidListStr)
                    {
                        if (deviceIdStr.Contains(s))
                        {
                            addDevice = true;
                            break;
                        }
                    }
                }
                else
                {
                    addDevice = true;
                }
                if (addDevice)
                {
                    keyValuePairs.Add(pnpDeviceID, device);
                }
            }
            return keyValuePairs;
        }



#if COMPILE
        public static List<USBDeviceInfo> RefreshDeviceList(UInt16[] vidList = null)
        {
            List<String> vidListStr = new List<String>();
            if (vidList != null)
            {
                foreach (UInt16 v in vidList)
                {
                    vidListStr.Add(String.Format("VID_{0:X4}", v));
                }
            }

            var pnpEntityList = GetPnpEntityList(vidListStr.ToArray());
            var usbControllerDeviceList = GetUsbControllerDeviceList(vidListStr.ToArray());
            ConnectedDeviceTree = CreateDetailedTree(usbControllerDeviceList, pnpEntityList);

            return ConnectedDeviceTree;
        }
#endif 

    }
}
