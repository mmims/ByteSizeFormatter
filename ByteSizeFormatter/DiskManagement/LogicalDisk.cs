using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace DiskManagement
{
    public class LogicalDisk : DiskElement
    {
        public enum DriveType
        {
            Unknown,
            NoRootDirectory,
            RemovableDisk,
            LocalDisk,
            NetworkDrive,
            CompactDisc,
            RAMDisk
        }

        protected LogicalDisk(string deviceID)
            : base(deviceID)
        {
        }

        internal static LogicalDisk GetLogicalDisk(string deviceID)
        {
            LogicalDisk logDisk = null;
            string[] filter = new string[] { "compressed", "drivetype", "filesystem", "freespace", "volumedirty", "volumename", "volumeserialnumber" };

            foreach (ManagementObject disk in new ManagementObjectSearcher(String.Format(
                "associators of {{Win32_DiskPartition.DeviceID='{0}'}} " +
                "where AssocClass = Win32_LogicalDiskToPartition", deviceID)).Get())
            {
                logDisk = new LogicalDisk(disk["DeviceID"].ToString());

                foreach (PropertyData property in disk.Properties)
                {
                    string name = property.Name.ToLowerInvariant();
                    if (name == "deviceid") continue;
                    if (name == "size")
                    {
                        logDisk.Size = (UInt64)property.Value;
                    }
                    else if (filter.Contains(name))
                    {
                        logDisk.AddProperty(property);
                    }
                }
                break;
            }

            return logDisk;
        }
    }

}
