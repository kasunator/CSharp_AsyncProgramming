using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WindowsSerialPorts;

namespace WindowsSerialPorts
{
    /* WHy We had to use the SETup API:
     * 
     * Problem and objective description.The bootlaoder and bootlaoder updater have the same USB PID ( product ID).
     * So when we try to update the bootlaoder we flash the bootloader updater to the application Flash location. 
     * Then we jump to the bootloader updater. From the running bootlaoder updater we erase and program the bootlaoder flash section.
     * Then we jump back to the bootloader and erase the application flash section( where the previous bootloader updater lives) and  program the final application.
     * Both the bootlaoder and the bootloader updater have the same PID. When we jump from the bootlaoder updater to the  bootlaoder. 
     * We don't know exactly if the bootlaoder updater disconnected and the device reconnected as the bootlaoder( at least this is the theory). 
     * We think that the device under update is still in the bootlaoder updater and we end up erasing the bootloader again and flashing the application to the bootlaoder Flash location, bricking the board in the process. 
     * To solve this issue we need to understand which mode the device is in( ie in bootlaoder or bootloader updater mode). 
     * Since both the bootlaoder and bootlaoder updater has the same PID we can only    verify this by using the device name. 
     * Our object is to use the windows setup API to get the USB device name ( "Bus Reported Device Description" to be exact) to verify that the device sucessfully made the transition from bootlaoder updater to bootlaoder.  
     * 
     * This code was developed using the inspiration received fromthe following links 
     * 
     * [The first example we had was in the following stack overflow link]
        (https://stackoverflow.com/questions/26732291/how-to-get-bus-reported-device-description-using-c-sharp)

	        This is in C# and made to get all the COM ports, it does get all the com ports  but the GetDEviceBusDescription() fails.

        [The second example I found was in the following stack overflow link]  

        (https://stackoverflow.com/questions/3438366/setupdigetdeviceproperty-usage-example)

	    This code is in C++. I was able to execute this code and I was able to sucessfully get the "Bus Reported Device Description" aka USB device name we were looking for.
     * 
     * */
    class SetupApiUSBInfo
    {
        const int utf16terminatorSize_bytes = 2;

        public enum StringLength : uint
        {
            MAX_PATH = 260,
        }


        public struct SetupApiUSBDevInfo
        {
            public string DeviceInstanceID;
            public string DeviceDescription;
            public string HardwareIDs;
            public string BusRprtedDevDesc;
            public string DeviceManufacturer;
            public string DeviceFriendlyName;
            public string DeviceLocationInfo;
            public string ContainerID;
            public string VID;
            public string PID;
            public string MI;
            public string port_name;

        }

        static SetupAPI.DEVPROPKEY DEVPKEY_Device_BusReportedDeviceDesc;
        static SetupAPI.DEVPROPKEY DEVPKEY_Device_Manufacturer;
        static SetupAPI.DEVPROPKEY DEVPKEY_Device_FriendlyName;
        static SetupAPI.DEVPROPKEY DEVPKEY_Device_LocationInfo;
        static SetupAPI.DEVPROPKEY DEVPKEY_Device_ContainerId;



        /* this functions resturns a list of SetuiApiUSBDevInfo for the comports that matches the passed pid and vids */
        public static List<SetupApiUSBDevInfo> GetComportInfoList(string vid, string[] pid_list)
        {
            StringBuilder devIDStrBuilder = new StringBuilder(260);
            /* populate the BusReportedDevieDesc with the corresponding GUID devpkey.h */

            //Guid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k);
            Guid comport_class = new Guid(0x4d36e978, 0xe325, 0x11ce, 0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18);

            List<SetupApiUSBDevInfo> devices = new List<SetupApiUSBDevInfo>();
            /* get an Hardware device info handle for devices enumarated as hid setup class */
            IntPtr hDeviceInfoSet = SetupAPI.SetupDiGetClassDevs(ref comport_class, IntPtr.Zero, IntPtr.Zero, SetupAPI.DiGetClassFlags.DIGCF_PRESENT);
            if (hDeviceInfoSet == IntPtr.Zero)
            {
                Console.WriteLine(" SetupDiGetClassDevs failed ");

            }

            try
            {
                UInt32 iMemberIndex = 0;
                while (true)
                {
                    SetupAPI.SP_DEVINFO_DATA deviceInfoData = new SetupAPI.SP_DEVINFO_DATA();
                    deviceInfoData.cbSize = (uint)Marshal.SizeOf(typeof(SetupAPI.SP_DEVINFO_DATA));
                    /* get the device information data structure i.e SP_DEVINFO_DATA from the device information set for the correspoinding device index */
                    bool success = SetupAPI.SetupDiEnumDeviceInfo(hDeviceInfoSet, iMemberIndex, ref deviceInfoData);
                    if (!success)
                    {
                        // No more devices in the device information set
                        break;
                    }

                    SetupApiUSBDevInfo deviceInfo = new SetupApiUSBDevInfo();


                    //deviceInfo.CM_Get_Device_ID;
                    if (GetDeviceID(deviceInfoData, out deviceInfo.DeviceInstanceID) != true)
                    {
                        continue;
                    }
                    /*Console.WriteLine("************************");*/

                    /*Console.WriteLine("CM_Get_Device_ID, aka DeviceInstanceID:{0}", deviceInfo.DeviceInstanceID);*/


                    //deviceInfo.DeviceDescription;
                    deviceInfo.DeviceDescription = GetDeviceDescription(hDeviceInfoSet, deviceInfoData);
                    /*Console.WriteLine("Device Description:{0}", deviceInfo.DeviceDescription);*/

                    //deviceInfo.HardwareIDs
                    deviceInfo.HardwareIDs = GetHardwareID(hDeviceInfoSet, deviceInfoData);

                    string[] hardWareIDArray = deviceInfo.HardwareIDs.Split('\0');
                    /*Console.WriteLine("HardwareIDs:");*/

                    //deviceInfo.BusRprtedDevDesc
                    deviceInfo.BusRprtedDevDesc = GetDeviceBusDescription(hDeviceInfoSet, deviceInfoData);
                    /*Console.WriteLine("Bus Reported Dev Descriptor:{0}", deviceInfo.BusRprtedDevDesc);*/


                    //deviceInfo.DeviceManufacturer
                    deviceInfo.DeviceManufacturer = GetDeviceManufacturer(hDeviceInfoSet, deviceInfoData);
                    /*Console.WriteLine("Device Manufacturer:{0}", deviceInfo.DeviceManufacturer);*/


                    //deviceInfo.DeviceFriendlyName
                    deviceInfo.DeviceFriendlyName = GetDeviceFriendlyName(hDeviceInfoSet, deviceInfoData);
                    /*Console.WriteLine("Device Friendly Name:{0}", deviceInfo.DeviceFriendlyName);*/

                    //deviceInfo.DeviceLocationInfo
                    deviceInfo.DeviceLocationInfo = GetDeviceLocationInfo(hDeviceInfoSet, deviceInfoData);
                    /*Console.WriteLine("Device location info:{0}", deviceInfo.DeviceLocationInfo);*/

                    //deviceInfo.ContainerID
                    deviceInfo.ContainerID = GetDeviceContainerID(hDeviceInfoSet, deviceInfoData);
                    /*Console.WriteLine("Device Container ID:{0}", deviceInfo.ContainerID);*/

                    string[] splitStringArray = deviceInfo.DeviceInstanceID.Split('&');
                    //deviceInfo.VID
                    /* we look for index of "VID_" then extract index+4 to end*/
                    if (splitStringArray.Length >= 1)
                    {
                        int VID_index = splitStringArray[0].IndexOf("VID_");
                        if (VID_index > -1)
                        {
                            deviceInfo.VID = splitStringArray[0].Substring(VID_index + "VID_".Length, 4);
                            /*Console.WriteLine("VID:{0}", deviceInfo.VID);*/
                        }
                    }
                    //deviceInfo.PID
                    /* we look for index of "PID_" then extract index+4 to end*/
                    if (splitStringArray.Length >= 2)
                    {
                        int PID_index = splitStringArray[1].IndexOf("PID_");
                        if (PID_index > -1)
                        {
                            deviceInfo.PID = splitStringArray[1].Substring(PID_index + "PID_".Length, 4);
                            /*Console.WriteLine("PID:{0}", deviceInfo.PID);*/
                        }
                    }

                    //deviceInfo.MI
                    /* we look for index of "MI_" then extract index+4 to end*/
                    if (splitStringArray.Length >= 3)
                    {
                        int MI_index = splitStringArray[2].IndexOf("MI_");
                        if (MI_index > -1)
                        {
                            deviceInfo.MI = splitStringArray[2].Substring(MI_index + "MI_".Length, 2);
                            /*Console.WriteLine("MI:{0}", deviceInfo.MI);*/
                        }
                    }

                    deviceInfo.port_name = GetPortName(hDeviceInfoSet, deviceInfoData);



                    /*Console.WriteLine("************************");*/
                    if (deviceInfo.PID != null && deviceInfo.VID != null)
                    {
                        foreach (string pid in pid_list)
                        {
                            /* the busreported device descriptor will be valid if the Device Description is == "USB Input Device" USB Input Device*/
                            if (vid.Contains(deviceInfo.VID) && pid.Contains(deviceInfo.PID)
                                /* deviceInfo.DeviceDescription.Contains("USB Input Device")) */
                                && deviceInfo.BusRprtedDevDesc != "")
                            {
                                devices.Add(deviceInfo);
                                break;
                            }
                        }
                    }

                    iMemberIndex++;
                }
            }
            finally
            {
                if (SetupAPI.SetupDiDestroyDeviceInfoList(hDeviceInfoSet) == true)
                {
                    //Console.WriteLine("Destroy Device Info List successful");
                }
                else
                {
                    //Console.WriteLine("Destroy Device Info List Failed");
                }
            }

            return devices;

        }


        private static bool GetDeviceID(SetupAPI.SP_DEVINFO_DATA devInfoData, out string device_id)
        {
            StringBuilder devIDStrBuilder = new StringBuilder((int)StringLength.MAX_PATH);
            if (SetupAPI.CM_Get_Device_ID(devInfoData.DevInst, devIDStrBuilder, (uint)StringLength.MAX_PATH, 0) != 0)
            {
                device_id = "";
                return false;
            }
            device_id = devIDStrBuilder.ToString();
            return true;
        }

        private static string GetDeviceDescription(IntPtr hDeviceInfoSet, SetupAPI.SP_DEVINFO_DATA deviceInfoData)
        {
            byte[] ptrBuf = new byte[SetupAPI.BUFFER_SIZE];
            uint propRegDataType;
            uint RequiredSize;
            bool success = SetupAPI.SetupDiGetDeviceRegistryProperty(hDeviceInfoSet, ref deviceInfoData, SetupAPI.SPDRP.SPDRP_DEVICEDESC,
            out propRegDataType, ptrBuf, SetupAPI.BUFFER_SIZE, out RequiredSize);
            if (!success)
            {
                // throw new Exception("Can not read registry value PortName for device " + deviceInfoData.ClassGuid);
                //Console.WriteLine("Can not read SetupDiGetDeviceRegistryProperty for SPDRP_DEVICEDESC " + deviceInfoData.ClassGuid);
                return "";
            }
            return Encoding.Unicode.GetString(ptrBuf, 0, (int)RequiredSize - utf16terminatorSize_bytes);
        }

        private static string GetHardwareID(IntPtr hDeviceInfoSet, SetupAPI.SP_DEVINFO_DATA deviceInfoData)
        {
            byte[] ptrBuf = new byte[SetupAPI.BUFFER_SIZE];
            uint propRegDataType;
            uint RequiredSize;
            bool success = SetupAPI.SetupDiGetDeviceRegistryProperty(hDeviceInfoSet, ref deviceInfoData, SetupAPI.SPDRP.SPDRP_HARDWAREID,
            out propRegDataType, ptrBuf, SetupAPI.BUFFER_SIZE, out RequiredSize);
            if (!success)
            {
                // throw new Exception("Can not read registry value PortName for device " + deviceInfoData.ClassGuid);
                //Console.WriteLine("Can not read SetupDiGetDeviceRegistryProperty for SPDRP_HARDWAREID " + deviceInfoData.ClassGuid);
                return "";
            }
            return Encoding.Unicode.GetString(ptrBuf, 0, (int)RequiredSize - utf16terminatorSize_bytes);
        }

        private static string GetDeviceBusDescription(IntPtr hDeviceInfoSet, SetupAPI.SP_DEVINFO_DATA deviceInfoData)
        {
            DEVPKEY_Device_BusReportedDeviceDesc = new SetupAPI.DEVPROPKEY();
            DEVPKEY_Device_BusReportedDeviceDesc.fmtid = new Guid(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2);
            DEVPKEY_Device_BusReportedDeviceDesc.pid = 4;

            byte[] ptrBuf = new byte[SetupAPI.BUFFER_SIZE];
            uint propRegDataType;
            uint RequiredSize;
            bool success = SetupAPI.SetupDiGetDevicePropertyW(hDeviceInfoSet, ref deviceInfoData, ref DEVPKEY_Device_BusReportedDeviceDesc,
            out propRegDataType, ptrBuf, SetupAPI.BUFFER_SIZE, out RequiredSize, 0);
            if (!success)
            {
                //throw new Exception("Can not read Bus provided device description device " + deviceInfoData.ClassGuid);
                //Console.WriteLine("Can not read SetupDiGetDevicePropertyW for DEVPKEY_Device_BusReportedDeviceDesc " + deviceInfoData.ClassGuid);
                return "";
            }
            return System.Text.UnicodeEncoding.Unicode.GetString(ptrBuf, 0, (int)RequiredSize - utf16terminatorSize_bytes);
        }

        private static string GetDeviceManufacturer(IntPtr hDeviceInfoSet, SetupAPI.SP_DEVINFO_DATA deviceInfoData)
        {
            DEVPKEY_Device_Manufacturer = new SetupAPI.DEVPROPKEY();
            DEVPKEY_Device_Manufacturer.fmtid = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0);
            DEVPKEY_Device_Manufacturer.pid = 13;

            byte[] ptrBuf = new byte[SetupAPI.BUFFER_SIZE];
            uint propRegDataType;
            uint RequiredSize;
            bool success = SetupAPI.SetupDiGetDevicePropertyW(hDeviceInfoSet, ref deviceInfoData, ref DEVPKEY_Device_Manufacturer,
            out propRegDataType, ptrBuf, SetupAPI.BUFFER_SIZE, out RequiredSize, 0);
            if (!success)
            {
                //throw new Exception("Can not read Bus provided device description device " + deviceInfoData.ClassGuid);
                //Console.WriteLine("Can not read SetupDiGetDevicePropertyW for DEVPKEY_Device_Manufacturer " + deviceInfoData.ClassGuid);
                return "";
            }
            return System.Text.UnicodeEncoding.Unicode.GetString(ptrBuf, 0, (int)RequiredSize - utf16terminatorSize_bytes);
        }

        private static string GetDeviceFriendlyName(IntPtr hDeviceInfoSet, SetupAPI.SP_DEVINFO_DATA deviceInfoData)
        {
            DEVPKEY_Device_FriendlyName = new SetupAPI.DEVPROPKEY();
            DEVPKEY_Device_FriendlyName.fmtid = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0);
            DEVPKEY_Device_FriendlyName.pid = 14;

            byte[] ptrBuf = new byte[SetupAPI.BUFFER_SIZE];
            uint propRegDataType;
            uint RequiredSize;
            bool success = SetupAPI.SetupDiGetDevicePropertyW(hDeviceInfoSet, ref deviceInfoData, ref DEVPKEY_Device_FriendlyName,
            out propRegDataType, ptrBuf, SetupAPI.BUFFER_SIZE, out RequiredSize, 0);
            if (!success)
            {
                //throw new Exception("Can not read Bus provided device description device " + deviceInfoData.ClassGuid);
                //Console.WriteLine("Can not read SetupDiGetDevicePropertyW for DEVPKEY_Device_FriendlyName " + deviceInfoData.ClassGuid);
                return "";
            }
            return System.Text.UnicodeEncoding.Unicode.GetString(ptrBuf, 0, (int)RequiredSize - utf16terminatorSize_bytes);
        }


        private static string GetDeviceLocationInfo(IntPtr hDeviceInfoSet, SetupAPI.SP_DEVINFO_DATA deviceInfoData)
        {
            DEVPKEY_Device_LocationInfo = new SetupAPI.DEVPROPKEY();
            DEVPKEY_Device_LocationInfo.fmtid = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0);
            DEVPKEY_Device_LocationInfo.pid = 15;

            byte[] ptrBuf = new byte[SetupAPI.BUFFER_SIZE];
            uint propRegDataType;
            uint RequiredSize;
            bool success = SetupAPI.SetupDiGetDevicePropertyW(hDeviceInfoSet, ref deviceInfoData, ref DEVPKEY_Device_LocationInfo,
            out propRegDataType, ptrBuf, SetupAPI.BUFFER_SIZE, out RequiredSize, 0);
            if (!success)
            {
                //throw new Exception("Can not read Bus provided device description device " + deviceInfoData.ClassGuid);
                //Console.WriteLine("Can not read SetupDiGetDevicePropertyW for DEVPKEY_Device_LocationInfo " + deviceInfoData.ClassGuid);
                return "";
            }
            return System.Text.UnicodeEncoding.Unicode.GetString(ptrBuf, 0, (int)RequiredSize - utf16terminatorSize_bytes);
        }

        private static string GetDeviceContainerID(IntPtr hDeviceInfoSet, SetupAPI.SP_DEVINFO_DATA deviceInfoData)
        {
            DEVPKEY_Device_ContainerId = new SetupAPI.DEVPROPKEY();
            DEVPKEY_Device_ContainerId.fmtid = new Guid(0x8c7ed206, 0x3f8a, 0x4827, 0xb3, 0xab, 0xae, 0x9e, 0x1f, 0xae, 0xfc, 0x6c);
            DEVPKEY_Device_ContainerId.pid = 2;

            byte[] ptrBuf = new byte[16];
            uint propRegDataType;
            uint RequiredSize;
            bool success = SetupAPI.SetupDiGetDevicePropertyW(hDeviceInfoSet, ref deviceInfoData, ref DEVPKEY_Device_ContainerId,
            out propRegDataType, ptrBuf, (uint)ptrBuf.Length, out RequiredSize, 0);
            if (!success)
            {
                //throw new Exception("Can not read Bus provided device description device " + deviceInfoData.ClassGuid);
                //Console.WriteLine("Can not read SetupDiGetDevicePropertyW for DEVPKEY_Device_ContainerId " + deviceInfoData.ClassGuid);
                return "";
            }

            Guid guid = new Guid(ptrBuf);
            return guid.ToString();
        }

        private static string GetPortName(IntPtr pDevInfoSet, SetupAPI.SP_DEVINFO_DATA deviceInfoData)
        {
            IntPtr hDeviceRegistryKey = SetupAPI.SetupDiOpenDevRegKey(pDevInfoSet, ref deviceInfoData,
            SetupAPI.DICS_FLAG_GLOBAL, 0, SetupAPI.DIREG_DEV, SetupAPI.KEY_QUERY_VALUE);
            if (hDeviceRegistryKey == IntPtr.Zero)
            {
                //throw new Exception("Failed to open a registry key for device-specific configuration information");
                //Console.WriteLine("Failed to open a registry key for device-specific configuration information");
                return "";
            }

            byte[] ptrBuf = new byte[SetupAPI.BUFFER_SIZE];
            uint length = (uint)ptrBuf.Length;
            try
            {
                uint lpRegKeyType;
                int result = SetupAPI.RegQueryValueEx(hDeviceRegistryKey, "PortName", 0, out lpRegKeyType, ptrBuf, ref length);
                if (result != 0)
                {
                    //throw new Exception("Can not read registry value PortName for device " + deviceInfoData.ClassGuid);
                    //Console.WriteLine("Can not read registry value PortName for device " + deviceInfoData.ClassGuid);
                    return "";
                }
            }
            finally
            {
                SetupAPI.RegCloseKey(hDeviceRegistryKey);
            }

            return Encoding.Unicode.GetString(ptrBuf, 0, (int)length - utf16terminatorSize_bytes);
        }

        private static Guid[] GetClassGUIDs(string className)
        {
            UInt32 requiredSize = 0;
            Guid[] guidArray = new Guid[1];

            bool status = SetupAPI.SetupDiClassGuidsFromName(className, ref guidArray[0], 1, out requiredSize);
            if (true == status)
            {
                if (1 < requiredSize)
                {
                    guidArray = new Guid[requiredSize];
                    SetupAPI.SetupDiClassGuidsFromName(className, ref guidArray[0], requiredSize, out requiredSize);
                }
            }
            else
                throw new System.ComponentModel.Win32Exception();

            return guidArray;
        }



    }
}