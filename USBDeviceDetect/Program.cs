using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Web;

namespace USBDeviceDetect
{
    internal class Program
    {
        public enum DeviceConnectionType_t
        {
            UNKNOWN,
            HID,
            WEBUSB,
            VCOMM,
        };

        static List<USBDeviceInfo> ConnectedDeviceTree = null;

        private static ushort[] Vid = { 0x1FC9 }; 
        static void Main(string[] args)
        {
            List<USBDeviceInfo> deviceInfoList;
            deviceInfoList =  RefreshDeviceList(Vid);
            
            int i = 0, j =0;
            foreach (USBDeviceInfo deviceInfo in deviceInfoList)
            {
                Console.WriteLine("===List item 1 {0} =======", i++);
                Console.WriteLine("Connection type: {0}", deviceInfo.ConnectionType.ToString());

                foreach (USBDeviceInfo childDevInfo in deviceInfo.Children)
                {
                    Console.WriteLine(" child size :{0}", childDevInfo.Children.Count);
                }
            }
           
            Console.ReadLine();
        }

        public static int GetConnectDevices()
        {


            return 0;
        }
        /* returns dictionary of < PNPDeviceID proterty string, ManagementBaseObject>  from  From Win32_PnPEntity 
         pnpList contains < PNPDeviceID string from Win32_PNPEntity->"PNPDeviceID", ManagementBaseObject from Win32_PNPEntity>  
         */
        private static Dictionary<string, ManagementBaseObject> GetPnpEntityList(string[] vidListStr = null)
        {
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity"))
                collection = searcher.Get();

            Dictionary<string, ManagementBaseObject> keyValuePairs = new Dictionary<string, ManagementBaseObject>();

            foreach (ManagementBaseObject device in collection)
            {
                bool addDevice = false;
                String pnpDeviceID = (string)device.GetPropertyValue("PNPDeviceID");
                Console.WriteLine("GetPnpEntityList pnpDeviceID: {0}", pnpDeviceID);
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

        /* get usb controller device list if they have the "Dependent" property from Win32_USBControllerDevice 
         * and they match the vid list put them in List<string> and return
         *  controllerDeviceList  contains deviceID string list from Win32_USBControllerDevice->"Dependent"->"DeviceID" */
        private static List<String> GetUsbControllerDeviceList(string[] vidListStr = null)
        {
            List<String> devIdList = new List<string>();
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBControllerDevice"))
                collection = searcher.Get();

            foreach (ManagementBaseObject device in collection)
            {
                String dep = (string)device.GetPropertyValue("Dependent");
                if (vidListStr != null)
                {
                    foreach (String s in vidListStr)
                    {
                        if (dep.Contains(s) || vidListStr == null)
                        {
                            Console.WriteLine("GetUsbControllerDeviceList Dependent: {0}", dep);
                            int devIdIndex = dep.IndexOf("DeviceID=") + 10;
                            string devId = dep.Substring(devIdIndex, dep.Length - devIdIndex - 1);
                            devId = System.Text.RegularExpressions.Regex.Unescape(devId); /* Value comes back with escape characters, remove them */
                            devIdList.Add(devId);
                            break;
                        }
                    }
                } 
                else
                {
                    Console.WriteLine("GetUsbControllerDeviceList Dependent: {0}", dep);
                    int devIdIndex = dep.IndexOf("DeviceID=") + 10;
                    string devId = dep.Substring(devIdIndex, dep.Length - devIdIndex - 1);
                    devId = System.Text.RegularExpressions.Regex.Unescape(devId); /* Value comes back with escape characters, remove them */
                    devIdList.Add(devId);
                    //break;
                }
            }

            
            return devIdList;
        }
        /* controllerDeviceList  contains deviceID string list from Win32_USBControllerDevice->"Dependent"->"DeviceID",
         pnpList contains < PNPDeviceID string from Win32_PNPEntity->"PNPDeviceID", ManagementBaseObject from Win32_PNPEntity> */
        private static List<USBDeviceInfo> CreateDetailedTree(List<String> controllerDeviceList, Dictionary<string, ManagementBaseObject> pnpList)
        {
            List<USBDeviceInfo> detailedTree = new List<USBDeviceInfo>();
            //List<string> detailedTree = new List<string>();

            USBDeviceInfo parentInfo = null;
            //string parentInfo = null;
            foreach (var devId in controllerDeviceList)
            {
                /* Check if this is a child also include HID devices as children */
                Boolean isChild = devId.Contains("&MI_") | devId.Contains(@"HID\");

                ManagementBaseObject pnpEntity;
                try
                {
                    /* pnpList is a Dictionary <string(device ID), managementBaseObject>,
                     * so pnpList[devId] returns the management base object */
                    pnpEntity = pnpList[devId]; 
                }
                catch (Exception e)
                {
                    continue;
                }

                if (isChild == false) /* Parent ID */
                    {
                    /* If we have a previous parent, commit the parent */
                    if (parentInfo != null)
                    {
                        /* Previous parent is done */
                        detailedTree.Add(parentInfo);
                        parentInfo = null;
                    }
                    parentInfo = new USBDeviceInfo(pnpEntity, DetermineConnectionType(pnpEntity));
                }
                else /* Child ID */
                {
                    if (parentInfo != null)
                    {
                        USBDeviceInfo childInfo = new USBDeviceInfo(pnpEntity, DetermineConnectionType(pnpEntity));
                        parentInfo.Children.Add(childInfo);
                    }
                }
            }
            /* Commit the last parent */
            if (parentInfo != null)
            {
                detailedTree.Add(parentInfo);
            }
            return detailedTree;
        }

        
        public static List<USBDeviceInfo> RefreshDeviceList(UInt16[] vidList = null)
        {
            List<String> vidListStr;
            if (vidList != null)
            {
                vidListStr = new List<String>();
                foreach (UInt16 v in vidList)
                {
                    vidListStr.Add(String.Format("VID_{0:X4}", v));
                }
                /* returns dictionary of < PNPDeviceID proterty string, ManagementBaseObject>   */
                Dictionary<string, ManagementBaseObject> pnpEntityList = GetPnpEntityList(vidListStr.ToArray());
                /*get usb controller device list if they have the "Dependent" property and they match the vid list put them in List<string> and return*/
                List<String> usbControllerDeviceList = GetUsbControllerDeviceList(vidListStr.ToArray());
                ConnectedDeviceTree = CreateDetailedTree(usbControllerDeviceList, pnpEntityList);
            }
            else
            {   /* returns dictionary of < PNPDeviceID proterty string, ManagementBaseObject>   */
                Dictionary<string, ManagementBaseObject> pnpEntityList = GetPnpEntityList();
                /*get usb controller device list if they have the "Dependent" property and they match the vid list put them in List<string> and return*/
                List <String> usbControllerDeviceList = GetUsbControllerDeviceList();
                ConnectedDeviceTree = CreateDetailedTree(usbControllerDeviceList, pnpEntityList);
            }

            return ConnectedDeviceTree;
        }


        private static DeviceConnectionType_t DetermineConnectionType(ManagementBaseObject device)
        {
            var type = DeviceConnectionType_t.UNKNOWN;

            String service;
            String devId;
            try
            {
                service = (String)device.GetPropertyValue("Service");
            }
            catch (Exception e)
            {
                service = "";
            }

            try
            {
                devId = (String)device.GetPropertyValue("DeviceID");
            }
            catch (Exception e)
            {
                devId = "";
            }

            if (service != null && service.Contains("WinUSB"))
            {
                type = DeviceConnectionType_t.WEBUSB;
            }
            else if (service != null && service.Contains("usbser"))
            {
                type = DeviceConnectionType_t.VCOMM;
            }
            else if (devId != null && devId.Contains(@"HID\"))
            {
                type = DeviceConnectionType_t.HID;
            }
            return type;
        }


        public class USBDeviceInfo
        {
            
            public USBDeviceInfo(ManagementBaseObject device, DeviceConnectionType_t ConnectionType)
            {
                this.PnPDevice = device;
                this.Children = new List<USBDeviceInfo>();
                this.ConnectionType = ConnectionType;
            }

            public ManagementBaseObject PnPDevice { get; private set; }
            public List<USBDeviceInfo> Children { get; private set; }
            public DeviceConnectionType_t ConnectionType { get; private set; }
            public static List<USBDeviceInfo> GetDeviceList()
            {
                return ConnectedDeviceTree;
            }
        }
    }
}
