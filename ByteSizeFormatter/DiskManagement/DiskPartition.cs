using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace DiskManagement
{
    public class DiskPartition : DiskElement
    {
        
        protected internal DiskPartition(string deviceID)
            : base(deviceID)
        {
        }

        public LogicalDisk LogicalDisk
        {
            get;
            private set;
        }

        internal static List<DiskPartition> GetDiskPartitions(string deviceID)
        {
            List<DiskPartition> partitions = new List<DiskPartition>();
            string[] filter = new string[] { "bootable", "bootpartition", "diskindex", "index", "name", "primarypartition", "type" };

            foreach (ManagementObject partition in new ManagementObjectSearcher(String.Format(
                        "associators of {{Win32_DiskDrive.DeviceID='{0}'}} " +
                        "where AssocClass = Win32_DiskDriveToDiskPartition",deviceID)).Get())
            {
                DiskPartition currentPartition = new DiskPartition(partition["DeviceID"].ToString());

                foreach (PropertyData property in partition.Properties)
                {
                    string name = property.Name.ToLowerInvariant();
                    if (name == "deviceid") continue;
                    if (name == "size")
                    {
                        currentPartition.Size = (UInt64)property.Value;
                    }
                    else if (filter.Contains(name))
                    {
                        currentPartition.AddProperty(property);
                    }
                }

                currentPartition.LogicalDisk = LogicalDisk.GetLogicalDisk(currentPartition.DeviceID);

                partitions.Add(currentPartition);
            }

            return partitions;
        }
    }
}
