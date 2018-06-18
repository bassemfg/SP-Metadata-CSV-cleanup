using System;
using System.Text;
using System.IO;
using System.Collections;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue headers = new Queue();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbFileRemoveSpaces = new StringBuilder();
            String[] columns = null;
            string line="";
            StreamReader sr = null;
            Stream stream = null;
            DirectoryInfo dir = new DirectoryInfo(@"c:\test\metadata");
            foreach (FileInfo f in dir.GetFiles())
            {
                stream = f.OpenText().BaseStream;
                sr = new StreamReader(stream);
                while (sr.Peek()>=0)
                {
                    line = sr.ReadLine();
                    if (line.Trim().Length > 0)
                    {
                        sbFileRemoveSpaces.Append(line);
                        sbFileRemoveSpaces.Append(@"
");
                    }
                    if (line.StartsWith("SourcePath"))
                    {
                       columns= line.Split(',');
                        foreach (string s in columns)
                        {
                            if (!headers.Contains(s))
                            {
                                headers.Enqueue(s);
                                sb.Append(s);
                                sb.Append(@"
");
                            }
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

        }
    }
}
