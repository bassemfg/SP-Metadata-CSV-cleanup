using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Data;

namespace ConsoleApp2
{
    class Program
    {
        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {


                string colName;
                int j = 0;
                string[] headers = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(sr.ReadLine())).Split(',');
                foreach (string header in headers)
                {
                    j++;
                    colName = header;
                    if (string.IsNullOrEmpty(header))
                        colName = "col" + j.ToString();

                    if (dt.Columns.Contains(header))
                        colName = header + j.ToString();

                    dt.Columns.Add(colName);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Trim().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(rows[i]))
                                dr[i] = rows[i].Replace("$^*", ",");
                        }
                        catch { }
                    }

                    if (!dr.HasErrors)//&& !dr.IsNull("FileRef"))
                        dt.Rows.Add(dr);
                }

            }


            return dt;
        }

        static void Main(string[] args)
        {

            int i = 0;
            DataTable dt = null;
            DataTable dtMerged = new DataTable("Metadata");
            DataRow dr = null;
            Hashtable ht = null;
            Queue headers = new Queue();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbFileRemoveSpaces = new StringBuilder();
            string line = "";
            //object[] columnNames = null;
            StreamReader sr = null;
            Stream stream = null;
            var VerifyOnly = true;

            string csvOutputFilePath = System.Configuration.ConfigurationSettings.AppSettings["csvOutputFilePath"];
            string xlsxFilesFolderPath = System.Configuration.ConfigurationSettings.AppSettings["xlsxFilesFolderPath"];
            
            DirectoryInfo dir = new DirectoryInfo(xlsxFilesFolderPath);
            int r = 0;

            foreach (FileInfo f in dir.GetFiles())
            {
                r = 0;
                try
                {
                    Console.WriteLine("Processing file " + f.Name);

                    if (VerifyOnly)
                    {
                        dt = ConvertCSVtoDataTable(f.FullName);
                        if (string.IsNullOrEmpty(dt.Rows[0]["UniqueId"].ToString()))
                        {
                            Console.WriteLine("No file uniqueid");
                            
                        }
                        else
                        { try { Guid g = new Guid(dt.Rows[0]["UniqueId"].ToString()); }
                            catch {
                                Console.WriteLine("No file uniqueid");
                            
                            }
                        }
                            
                        
                    }
                    else
                    {
                        stream = f.OpenText().BaseStream;
                        sr = new StreamReader(stream);
                        while (sr.Peek() >= 0)
                        {
                            r++;
                            //read a line in the CSV file
                            line = sr.ReadLine();
                            if (line.Trim().Length > 0)
                            {
                                //line = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(line));
                                // if it is not a header, i.e not start of a new table, add a new row to target
                                if (r == 1 && !line.StartsWith("ServerRedirectedEmbedUri"))
                                {
                                    Console.WriteLine("Not a metadata file");
                                    break;
                                }

                                if (r == 1 && line.StartsWith("ServerRedirectedEmbedUri"))
                                    sb = new StringBuilder();

                                if (r > 1 && line.StartsWith("ServerRedirectedEmbedUri"))
                                {
                                    SaveFile(f.FullName, sb.ToString());
                                    sb = new StringBuilder();
                                }

                                sb.Append(line);
                                sb.Append(@"
");
                            }


                            Console.WriteLine("In Row: " + r.ToString() + " file " + f.Name);
                        }

                        SaveFile(f.FullName, sb.ToString());

                        stream.Close();
                        sr.BaseStream.Close();

                        sr.Close();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("error: " + e.Message);
                    System.Diagnostics.EventLog.WriteEntry("SP-CSV-Cleanup", e.Message, System.Diagnostics.EventLogEntryType.Error, 123);
                }
            }
        }

        static void SaveFile(string filename, string contents)
        {


            StreamWriter sw = new StreamWriter(filename + DateTime.Now.ToFileTimeUtc().ToString() + @".csv");
            sw.Write(contents);
            sw.Flush();
            sw.Close();
            
        }
    }
}