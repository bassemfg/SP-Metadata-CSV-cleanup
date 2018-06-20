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
        static void Main(string[] args)
        {
            int i = 0;
            DataTable dt = new DataTable("Metadata");
            DataRow dr = null;
            Queue headers = new Queue();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbFileRemoveSpaces = new StringBuilder();
            String[] columns = null;
            string line = "";
            //object[] columnNames = null;
            StreamReader sr = null;
            Stream stream = null;
            DirectoryInfo dir = new DirectoryInfo(@"c:\test\metadata");
            foreach (FileInfo f in dir.GetFiles())
            {
                stream = f.OpenText().BaseStream;
                sr = new StreamReader(stream);
                while (sr.Peek() >= 0)
                {
                    line = sr.ReadLine();
                    if (!line.StartsWith("SourcePath"))
                        dt.Rows.Add(dt.NewRow());
                            
                    if (line.Trim().Length > 0)
                    {

                        sbFileRemoveSpaces.Append(line);
                        sbFileRemoveSpaces.Append(@"
");
                        columns = line.Split(',');
                        i = 0;
                        foreach (string s in columns)
                        {

                            if (line.StartsWith("SourcePath"))
                            {
                                if(!dt.Columns.Contains(s))
                                    dt.Columns.Add(s);
                                else
                                    if (!dt.Columns.Contains(s+i.ToString()))
                                        dt.Columns.Add(s+i.ToString());


                                if (!headers.Contains(s))
                                {
                                    headers.Enqueue(s);
                                
                                    sb.Append(s);
                                    sb.Append(@"
");
                                }
                            }

                            else
                            {
                                if (i < dt.Columns.Count )
                                    dt.Rows[dt.Rows.Count - 1][i] = s;//dt.Columns[i].ColumnName
                            }

                            i++;
                        }
                    }
                }
                stream.Close();
                sr.BaseStream.Close();

                sr.Close();
                sbFileRemoveSpaces.Append(@"
");
            }

            StreamWriter sw = new StreamWriter(@"c:\test\columns.txt");
            sw.Write(sb.ToString());
            sw.Flush();
            sw.Close();

            sw = new StreamWriter(@"c:\test\allmetadata.csv");
            sw.Write(sbFileRemoveSpaces.ToString());
            sw.Flush();
            sw.Close();

            dt.WriteXml(@"c:\test\allmetadata.xml");

        }
    }
}
