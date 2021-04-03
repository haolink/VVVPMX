/**
 * Copyright (C) 2021 haolink <https://www.twitter.com/haolink> / <https://github.com/haolink>
 * Code based on chrrox Orochi4 converter
 * 
 * Code licensed under Apache 2.0 license, see LICENSE
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.InteropServices;

using OrochiPMX;

namespace VVVTune2PMX
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

            if (args.Length < 1)
            {
                Console.WriteLine("Requires input file parameter");
                System.Threading.Thread.Sleep(1000);
                return;
            }

            string inputFile = args[0];

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Input file not found");
                System.Threading.Thread.Sleep(1000);
                return;
            }

            string fullPathInputFile = Path.GetFullPath(inputFile);
            string extension = Path.GetExtension(fullPathInputFile).ToLowerInvariant();

            string mdlFile = null;
            string mdcFile = null;
            if (extension == ".mdl" || extension == ".fa61c1d7")
            {
                mdlFile = fullPathInputFile;
            }
            if (extension == ".mdc" || extension == ".5e12de0d")
            {
                mdcFile = fullPathInputFile;
            }

            if (mdcFile == null && mdlFile == null)
            {
                Console.WriteLine("Unable to detect file format - please make sure the extension is recognisable (mdl, mdc, fa61c1d7, 5e12de0d)");
                System.Threading.Thread.Sleep(1000);
                return;
            }



            if (mdlFile != null)
            {
                string tMDC = Path.GetDirectoryName(mdlFile) + @"\..\5e12de0d\" + Path.GetFileNameWithoutExtension(mdlFile);
                
                if (File.Exists(tMDC + ".mdc"))
                {
                    mdcFile = Path.GetFullPath(tMDC + ".mdc");
                }
                if (File.Exists(tMDC + ".5e12de0d"))
                {
                    mdcFile = Path.GetFullPath(tMDC + ".5e12de0d");
                }
            }
            else
            {
                string tMDL = Path.GetDirectoryName(mdcFile) + @"\..\fa61c1d7\" + Path.GetFileNameWithoutExtension(mdcFile);

                if (File.Exists(tMDL + ".mdl"))
                {
                    mdlFile = Path.GetFullPath(tMDL + ".mdl");
                }
                if (File.Exists(tMDL + ".fa61c1d7"))
                {
                    mdlFile = Path.GetFullPath(tMDL + ".fa61c1d7");
                }
            }

            if (mdlFile == null)
            {
                Console.WriteLine("Cannot identify model file.");
                System.Threading.Thread.Sleep(1000);
                return;
            }

            Console.WriteLine(" Input MDL file: " + mdlFile);
            if (mdcFile != null)
            {
                Console.WriteLine(" Input MDC file: " + mdcFile);
            }

            string pmxFile = Path.GetFullPath(Path.ChangeExtension(mdlFile, ".pmx"));

            Console.WriteLine("Output PMX file: " + pmxFile);

            try
            {
                GsmdlFile gsmdl = new GsmdlFile(
                    mdlFile,
                    mdcFile,
                    pmxFile
                );
                if (mdcFile != null)
                {
                    gsmdl.loadMdc();
                }
                gsmdl.loadMdlHeader();

                Console.WriteLine("\r\nConversion complete!");
            } 
            catch (Exception ex)
            {
                Console.WriteLine("\r\nError occured: " + ex.Message);
            }

            System.Threading.Thread.Sleep(1000);            
        }
    }
}
