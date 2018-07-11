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
            string Val = "";
            DataTable dt = null;
            DataTable dtMerged = new DataTable("Metadata");
            DataRow dr = null;
            Hashtable ht = null;
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
                    //read a line in the CSV file
                    line = sr.ReadLine();
                    if (line.Trim().Length > 0)
                    {
                        // if it is not a header, i.e not start of a new table, add a new row to target
                        if (!line.StartsWith("SourcePath"))
                            dt.Rows.Add(dt.NewRow());
                        else //if it is a header i.e. start of new table
                        {
                            // merge with existing table, if it exists
                            if (dt != null)
                                dtMerged.Merge(dt, true, MissingSchemaAction.Add);
                            ht = new Hashtable();
                            dt = new DataTable();

                        }


                        sbFileRemoveSpaces.Append(line);
                        sbFileRemoveSpaces.Append(@"
");
                        columns = line.Split(',');
                        i = 0;

                        foreach (string s in columns)
                        {

                            if (line.StartsWith("SourcePath"))
                            {
                                if (dt.Columns.Contains(s) == false)
                                {
                                    dt.Columns.Add(s);
                                    ht.Add(i, s);
                                }
                                else // to solve the problem of columns with same name
                                {
                                    if (dt.Columns.Contains(s + i.ToString()) == false)
                                    {
                                        dt.Columns.Add(s + i.ToString());
                                        ht.Add(i, s + i.ToString());
                                    }
                                }


                                if (headers.Contains(s) == false)
                                {
                                    headers.Enqueue(s);

                                    sb.Append(s);
                                    sb.Append(@"
");
                                }
                            }

                            else
                            {
                                Val = s;
                                if (s.IndexOf('#') > 0)
                                    Val = s.Substring(1 + s.IndexOf('#'));

                                if (i < dt.Columns.Count && !string.IsNullOrEmpty(ht[i].ToString()))
                                    dt.Rows[dt.Rows.Count - 1][ht[i].ToString()] = Val;//dt.Columns[i].ColumnName
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
            /*
                        sw = new StreamWriter(@"c:\test\allmetadata.csv");
                        sw.Write(sbFileRemoveSpaces.ToString());
                        sw.Flush();
                        sw.Close();
                        */
            dtMerged.WriteXml(@"c:\test\allmetadata.xml");
        }

    }
}

