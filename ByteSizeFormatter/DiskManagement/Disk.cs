using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace DiskManagement
{
    public class Disk : DiskElement
    {
        #region Private Members

        private List<DiskPartition> _partitions;

        #endregion Private Members

        protected Disk(string deviceId)
            : base(deviceId)
        {
            _partitions = new List<DiskPartition>();
        }

        #region Properties

        public List<DiskPartition> Partitions
        {
            get { return _partitions; }
            private set { _partitions = value; }
        }

        #endregion Properties

        #region Static Methods

        public static List<Disk> GetDisks(DiskType type = DiskType.All)
        {
            List<Disk> disks = new List<Disk>();
            string[] filter = new string[] { "manufacturer", "mediatype", "model", "partitions", "serialnumber"};

            foreach (ManagementObject disk in new ManagementObjectSearcher("Select * From Win32_DiskDrive").Get())
            {
                if (type == DiskType.All || disk["MediaType"].ToString().ToLowerInvariant().Contains(type.ToString().ToLowerInvariant()))
                {
                    Disk currentDisk = new Disk(disk["DeviceID"].ToString());

                    foreach (PropertyData property in disk.Properties)
                    {
                        string name = property.Name.ToLowerInvariant();
                        if (name == "deviceid") continue;
                        if (name == "size")
                        {
                            currentDisk.Size = (UInt64)property.Value;
                        }
                        else if (filter.Contains(name))
                        {
                            currentDisk.AddProperty(property);
                        }
                    }

                    // add associated disk partitions
                    currentDisk.Partitions = DiskPartition.GetDiskPartitions(currentDisk.DeviceID);

                    disks.Add(currentDisk);
                }
            }

            return disks;
        }

        #endregion Static Methods
    }

    public enum DiskType
    {
        All,
        External,
        Fixed,
        Removable
    }
}
