using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.InteropServices;

namespace OrochiPMX
{       

    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCP(uint wCodePageID);

        static void Main(string[] args)
        {
            SetConsoleOutputCP(65001);
            SetConsoleCP(65001);
            Console.OutputEncoding = Encoding.UTF8;

            string inputExtracts = @"F:\rip\nvs\";
            string outputFolder = @"F:\rip\nvs\convert\more\";

            List<string> archFolders = new List<string>();

            for (int i = 0; i < 100; i++)
            {
                string fld = inputExtracts + "GxArchivedFile" + String.Format("{0:000}", i) + @".csh_extract\";
                if (Directory.Exists(fld))
                {                    
                    archFolders.Add(fld);
                }
            }

            SortedList<string, List<string>> assignments = new SortedList<string, List<string>>();

            foreach (string fld in archFolders)
            {
                string mtDataFolder = fld + "C457B87E";
                if (!Directory.Exists(mtDataFolder))
                {
                    continue;
                }

                string[] mtDataFiles = Directory.GetFiles(mtDataFolder, "*.C457B87E");

                foreach (string mtDataFile in mtDataFiles)
                {
                    byte[] buffer = File.ReadAllBytes(mtDataFile);

                    List<long> offsets = buffer.IndexesOf(Encoding.ASCII.GetBytes("model:MODEL/CHARA/"));
                    if (offsets.Count > 0)
                    {
                        int off = (int)offsets[0];
                        int length = 0;
                        while (buffer[off + 18 + length] != 0x2E)
                        {
                            length++;
                        }
                        if (length <= 0)
                        {
                            continue;
                        }
                        byte[] mdlNameBuf = new byte[length];
                        Buffer.BlockCopy(buffer, off + 18, mdlNameBuf, 0, length);

                        string mdlName = Encoding.ASCII.GetString(mdlNameBuf);

                        List<string> mdlParts = mdlName.Split(new char[] { '/' }).ToList();
                        mdlParts.RemoveAt(mdlParts.Count - 1);

                        string outFolder = String.Join("\\", mdlParts.ToArray());

                        if (!assignments.ContainsKey(outFolder))
                        {
                            assignments.Add(outFolder, new List<string>());
                        }
                        assignments[outFolder].Add(Path.GetFileNameWithoutExtension(mtDataFile));                        
                    }
                }
            }

            foreach (KeyValuePair<string, List<string>> kvp in assignments)
            {
                kvp.Value.Sort();
                /*Console.WriteLine(kvp.Key + ": ");
                foreach (string assignment in kvp.Value)
                {
                    Console.WriteLine("  " + assignment);
                }
                Console.WriteLine();*/
            }

            
            foreach (KeyValuePair<string, List<string>> kvp in assignments)
            {
                string charCode = kvp.Key;
                Console.WriteLine(charCode + ":");

                foreach (string mdlCode in kvp.Value)
                {
                    string[] mdlFiles = Directory.GetFiles(inputExtracts, mdlCode + ".mdl", SearchOption.AllDirectories);
                    if (mdlFiles.Length == 0)
                    {
                        Console.WriteLine(" - " + mdlCode + "... not found!");
                    }

                    for (int i = 0; i < mdlFiles.Length; i++)
                    {
                        string mdlFile = mdlFiles[i];
                        string texDir = mdlFile.Substring(mdlFile.IndexOf("GxArchivedFile") + 14, 3);

                        string outFilePmx = mdlCode + "_T" + texDir;
                        if (i > 0)
                        {
                            outFilePmx += "_I" + String.Format("{0:000}", i);
                        }

                        string mdc = null;
                        string expectedMdc = Path.GetFullPath(Path.GetDirectoryName(mdlFile) + @"\..\5E12DE0D\" + mdlCode + ".mdc");

                        Console.Write(" - " + mdlCode + "... ");
                        if (File.Exists(expectedMdc))
                        {
                            mdc = expectedMdc;
                            Console.Write("mdc ok... ");
                        } else
                        {
                            Console.Write("mdc missing... ");
                        }

                        string outFolder = outputFolder + charCode + @"\";
                        if (!Directory.Exists(outFolder))
                        {
                            Directory.CreateDirectory(outFolder);
                        }

                        try
                        {
                            GsmdlFile gsmdlFile = new GsmdlFile(
                                mdlFile,
                                mdc,
                                outFolder + outFilePmx + ".pmx"
                            );

                            gsmdlFile.loadMdc();
                            gsmdlFile.loadMdlHeader();
                            Console.WriteLine("mdl okay.");
                        } catch(Exception ex)
                        {
                            StreamWriter sw = File.CreateText(outFolder + outFilePmx + ".fail");
                            sw.Write("");
                            sw.Close();
                            sw = null;
                            Console.WriteLine("mdl fail: " + ex.Message);
                        }
                    }
                }
                Console.WriteLine("");
            }

            Console.ReadLine();

            /*string outfldr = "mdl_c830";

            string[] files = new string[] { "5257D25A", "F7E1AFA2"/*, "74EEFF02", "D31AC958", "6DC08766", "A0856AB2", "E479CFC1" *//*};

            foreach (string file in files)
            {
                GsmdlFile gsmdlFile = new GsmdlFile(
                    @"F:\rip\nvs\GxArchivedFile054.csh_extract\FA61C1D7\" + file + ".mdl",
                    @"F:\rip\nvs\GxArchivedFile054.csh_extract\5E12DE0D\" + file + ".mdc",
                    @"F:\rip\nvs\GxArchivedFile054.csh_extract\" + outfldr + @"\" + file +".pmx");

                    gsmdlFile.loadMdc();
                    gsmdlFile.loadMdlHeader();
            }

            Console.ReadLine();*/
            
            /*GsmdlFile gsmdlFile = new GsmdlFile(
                @"F:\rip\nvs\GxArchivedFile054.csh_extract\FA61C1D7\D157894C.mdl",
                @"F:\rip\nvs\GxArchivedFile054.csh_extract\5E12DE0D\D157894C.mdc",
                @"F:\rip\nvs\GxArchivedFile054.csh_extract\FA61C1D7\D157894C_2.pmx");*/            
        }
    }
}
