using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Xpath
{
    class Program
    {
        /*
         You may start downloads from command line using command line parameters
        You may start IDM from the command line using the following parameters

        idman /s
        or idman /d URL [/p local_path] [/f local_file_name] [/q] [/h] [/n] [/a]

        Parameters:
        /d URL - downloads a file, eg.
        IDMan.exe /d "http://www.internetdownloadmanager.com/path/File Name.zip"
        /s - starts queue in scheduler
        /p local_path - defines the local path where to save the file
        /f local_file_name - defines the local file name to save the file
        /q - IDM will exit after the successful downloading. This parameter works only for the first copy
        /h - IDM will hang up your connection after the successful downloading
        /n - turns on the silent mode when IDM doesn't ask any questions
        /a - add a file, specified with /d, to download queue, but don't start downloading

        Parameters /a, /h, /n, /q, /f local_file_name, /p local_path work only if you specified the file to download with /d URL
             */

       
        public static void Main(string[] args)
        {


            try
            {
                //var s = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\Config.ini");
                //testc(s[0], int.Parse(s[1]), int.Parse(s[2]));
                //Console.WriteLine("Done");
                //Console.ReadKey();

                //vnex();
                //SaveVideo("http://video.vnexpress.net/cuoi/hai-nguoi-dan-khiep-dam-khi-nu-phong-vien-bien-hinh-3401099.html");

                //MoveVideo();
                //Copy(@"D:\Temp\vnex", @"D:\Temp\Clips");
                //GetInfoItems();
                Tastmade.SaveInfoTM();
                //GetLinkTMUS();
                //GetLinkTM();
                //teet();

                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
