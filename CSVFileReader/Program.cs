﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace CVSFileReader
{
    internal class Program
    {
        /*
         * "nid_universal_updater_record",
         * "ctx_microcontroller_sn",
         * "ctx_serialization",
         * "ctx_coding_pass_fail",
         * "ddt_time_programmed","ctx_machine_name","ctx_company_name","nid_universal_updater_product","nid_release_file_storage","ctx_product_name","nid_universal_updater_records_testing","ctx_release_version"]
         * 
         */

        public class prog_record{
            public UInt64 nid_universal_updater_record { get; set;  }
            public String ctx_microcontroller_sn { get; set; }
            public UInt64 ctx_serialization { get; set; } 
            public UInt64 ctx_coding_pass_fail { get; set; }
            public string ddt_time_programmed { get; set; }
            public string ctx_machine_name { get; set; }
            public string ctx_company_name { get; set; }
            public string nid_universal_updater_product { get; set; }
            public string nid_release_file_storage { get; set; }
            public string ctx_product_name { get; set; }
            public string nid_universal_updater_records_testing { get; set; }
            public string ctx_release_version { get; set; }
        }
        public static void Main(string[] args)
        {
            var filePath = "rpeated_sn.csv";
            var records = ReadCsvFile(filePath);
            List<UInt64> reptd_ser_number = new List<UInt64>();
            List< prog_record > reptd_ser_record = new List<prog_record>();
            List<prog_record> collided_ser_record = new List<prog_record>();

            foreach (var record in records)
            {
                /*if (record.ctx_serialization == 9065620524302389)
                {
                    Console.WriteLine(record.nid_universal_updater_record);
                    
                }*/
                /* first find the repeated serial numbers*/
                foreach (var inner in records)
                {
                    if (inner.ctx_serialization == record.ctx_serialization && inner.ctx_machine_name != record.ctx_machine_name)
                    {
                        DateTime recordDateTime = DateTime.Parse(record.ddt_time_programmed);
                        DateTime innerDateTime = DateTime.Parse(inner.ddt_time_programmed);
                        if (innerDateTime.Year == recordDateTime.Year && innerDateTime.Month == recordDateTime.Month && innerDateTime.Day == recordDateTime.Day
                            && innerDateTime.Hour == recordDateTime.Hour && innerDateTime.Minute == recordDateTime.Minute )
                        {
                            int seconds_diff = Math.Abs( recordDateTime.Second - innerDateTime.Second);
                            if (seconds_diff < 10)
                            {
                                reptd_ser_number.Add(inner.ctx_serialization);
                                reptd_ser_record.Add(inner);
                            }
                        }

                    }
                }
            }
            Console.WriteLine("reptd_ser_number count: " + reptd_ser_number.Count);
            Console.WriteLine("reptd_ser_record count: " + reptd_ser_record.Count);
#if COMPILE
            foreach (var record in reptd_ser_record)
            {
                foreach(var inner in reptd_ser_record)
                {
                    if (record.ctx_serialization == inner.ctx_serialization)
                    {
                        if (record.ctx_microcontroller_sn != inner.ctx_microcontroller_sn)
                        {
                            collided_ser_record.Add(record);
                        }
                    }
                }
                
            }
#endif 
            foreach(var record in reptd_ser_record)
            {
                Console.WriteLine("reptd_ser_record serialization: " + record.ctx_serialization);
            }


            //Console.WriteLine("collided_ser_record count: " + collided_ser_record.Count);

            Console.ReadLine();
        }

        public static List<prog_record> ReadCsvFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return new List<prog_record>(csv.GetRecords<prog_record>());
            }
        }

    }
}
