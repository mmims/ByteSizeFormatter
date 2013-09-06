using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Text.RegularExpressions;
using DiskManagement;

namespace ByteSizeFormatter
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (Disk disk in Disk.GetDisks())
            {
                Console.WriteLine("-Disk ({0})", disk.DeviceID);
                foreach (var property in disk.Properties)
                {
                    Console.WriteLine("  {0,-20}: {1}", property.Key, property.Value.ToString());
                }
                Console.WriteLine("  {0,-20}: {1}", "Size", disk.GetSizeString());

                foreach (DiskPartition part in disk.Partitions)
                {
                    Console.WriteLine(" -Partition ({0})", part.DeviceID);
                    foreach (var property in part.Properties)
                    {
                        Console.WriteLine("    {0,-20}: {1}", property.Key, property.Value.ToString());
                    }
                    Console.WriteLine("    {0,-20}: {1}", "Size", part.GetSizeString());

                    if (part.LogicalDisk != null)
                    {
                        Console.WriteLine("   -Logical Disk ({0})", part.LogicalDisk.DeviceID);
                        foreach (var property in part.LogicalDisk.Properties)
                        {
                            Console.WriteLine("      {0,-20}: {1}", property.Key, property.Value.ToString());
                        }
                        Console.WriteLine("      {0,-20}: {1}", "Size", part.LogicalDisk.GetSizeString());
                    }
                }
                
            }
        }
    }
} 
