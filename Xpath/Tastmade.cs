using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using HtmlAgilityPack;

namespace Xpath
{
    internal class Tastmade
    {
        public static void SaveInfoTM()
        {
            // save item info(video, recip)
            var count = 0;
            var config = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\config.txt");
            var listLinks = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\Input\\tastemade.txt");
            foreach (var link in listLinks)
            {
                var process = new Process();
                //Process.Start("C:\\Program Files (x86)\\Internet Download Manager\\IDMan.exe", " /d http://images.contentful.com/pxqrocxwsjcc/3imbjBXYRaaccgomkGk8aG/478d47e7b12f91cd5bca7ab292a6924c/chips-three-ways_landscapeThumbnail_en.png?w=960&fl=progressive&fm=jpg");
                if (string.IsNullOrWhiteSpace(link)) continue;
                string html = null;
                var webClient = new WebClient();
                var temp = link.Split('/');
                //var path = Directory.GetCurrentDirectory() + "\\Output\\" + temp[temp.Length - 1];
                var path = Directory.GetCurrentDirectory() + "\\Output";
                //Process.Start("C:\\Program Files (x86)\\Internet Download Manager\\IDMan.exe", " /d http://images.contentful.com/pxqrocxwsjcc/3imbjBXYRaaccgomkGk8aG/478d47e7b12f91cd5bca7ab292a6924c/chips-three-ways_landscapeThumbnail_en.png?w=960&fl=progressive&fm=jpg /p "+path);
                //if (Directory.Exists(path)) return;
                Directory.CreateDirectory(path);
                var writer = new StreamWriter(path + "\\" + temp[temp.Length - 1] + ".txt");
                var connection = NetworkInterface.GetIsNetworkAvailable();
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
                while (!connection)
                {
                    System.Threading.Thread.Sleep(5000);
                    Console.WriteLine("Internet not available! Waits");
                    connection = NetworkInterface.GetIsNetworkAvailable();
                }
                using (var stream = webClient.OpenRead(new Uri(link)))
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            html = reader.ReadToEnd();
                        }
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var curenPath = "//*[@class='VideoRecipe h-recipe']";
                var nodes = doc.DocumentNode.SelectNodes(curenPath);
                if (nodes == null)
                {
                    writer.Close();
                    continue;
                }
                var x = nodes[0].FirstChild.InnerText;
                writer.WriteLine(config[1] + x);
                writer.WriteLine(link);
                x = nodes[0].OuterHtml;
                writer.WriteLine(x);
                writer.WriteLine("\r###########################################\r");
                curenPath = "//*[@id='serverContext']";
                nodes = doc.DocumentNode.SelectNodes(curenPath);
                x = nodes[0].OuterHtml;
                var tempsds = new List<string>(x.Split(','));
                for (var i = 0; i < tempsds.Count; i++)
                {
                    if (tempsds[i].Contains("awsOriginal")) continue;
                    tempsds.RemoveAt(i);
                    i = i - 1;
                }
                var ll = tempsds[0].Substring(15, tempsds[0].Length - 16);
                writer.WriteLine(ll);
                //Process.Start(config[0], " /a /n /q /d " + ll + " /p " + path);
                writer.Close();
                Console.WriteLine(count++ + " of " + listLinks.Length + ": " + link);
                process = Render(process, ll, path + "\\" + temp[temp.Length - 1] + ".mp4");
                process.WaitForExit();
                process.Close();

            }
            //Process.Start("C:\\Program Files (x86)\\Internet Download Manager\\IDMan.exe", " /s ");
        }

        public static void GetLinkTMUS()
        {
            // Get all categories
            var listCategories = new List<string>();
            string html = null;
            var webClient = new WebClient();
            var path = Directory.GetCurrentDirectory() + "\\Output\\categories.txt";
            //Directory.CreateDirectory(path);
            var writer = new StreamWriter(path);
            var bNext = true;
            var i = 1;
            while (bNext)
            {
                var allCategories = "https://www.tastemade.com/recipes/categories?page=" + i;

                using (var stream = webClient.OpenRead(new Uri(allCategories)))
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            html = reader.ReadToEnd();
                        }
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var curenPath = "//a[@class='media-landscape-poster-thumb']";
                var nodes = doc.DocumentNode.SelectNodes(curenPath);
                listCategories.AddRange(nodes.Select(node => "https://www.tastemade.com" + node.Attributes[1].Value + "?page="));
                //curenPath = "//div[@class='right']";// US
                curenPath = "//div[@class='left']";// Uk
                nodes = doc.DocumentNode.SelectNodes(curenPath);
                if (nodes == null)
                    bNext = false;
                else
                    i++;
            }
            //List<string> videos = new List<string>();
            //Get all item in catologi

            foreach (var cate in listCategories)
            {
                i = 1;
                bNext = true;
                while (bNext)
                {
                    using (var stream = webClient.OpenRead(new Uri(cate + i)))
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                html = reader.ReadToEnd();
                            }
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var curenPath = "//a[@class='media-landscape-poster-thumb']";
                    var nodes = doc.DocumentNode.SelectNodes(curenPath);
                    foreach (var node in nodes)
                        writer.WriteLine("https://www.tastemade.com" + node.Attributes[1].Value);
                    //curenPath = "//div[@class='right']";// US
                    curenPath = "//div[@class='left']";// Uk
                    nodes = doc.DocumentNode.SelectNodes(curenPath);
                    if (nodes == null)
                        bNext = false;
                    else
                    {
                        i++;
                    }
                }

            }

            //writer.WriteLine(ll);

            writer.Close();
        }

        public static void GetLinkTM()
        {
            string link;
            // link = "https://www.tastemade.co.uk/recipes?page=";//UK
            // link = "https://www.tastemade.com.br/recipes?page="; //bra
            // link = "https://es.tastemade.com/recipes?page="; //es
            // link = "https://id.tastemade.com/recipes?page="; //id
            link = "https://www.tastemade.fr/recipes?page="; //fr
            var webClient = new WebClient();
            //string path = Directory.GetCurrentDirectory() + "\\Output\\UK_categories.txt";
            //string path = Directory.GetCurrentDirectory() + "\\Output\\BR_categories.txt";
            //string path = Directory.GetCurrentDirectory() + "\\Output\\ID_categories.txt";
            var path = Directory.GetCurrentDirectory() + "\\Output\\FR_categories.txt";
            //Directory.CreateDirectory(path);
            var writer = new StreamWriter(path);
            var bNext = true;
            var i = 1;

            while (bNext)
            {
                string html = null;
                using (var stream = webClient.OpenRead(new Uri(link + i)))
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            html = reader.ReadToEnd();
                        }
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var curenPath = "//a[@class='media-landscape-poster-thumb']";
                var nodes = doc.DocumentNode.SelectNodes(curenPath);
                foreach (var node in nodes)
                    writer.WriteLine("https://www.tastemade.fr" + node.Attributes[1].Value);
                curenPath = "//div[@class='left']";
                nodes = doc.DocumentNode.SelectNodes(curenPath);
                if (nodes == null)
                    bNext = false;
                else
                {
                    i++;
                }
                Console.WriteLine(i);
            }
            writer.Close();
        }

        public static Process Render(Process process, string link, string output)
        {
            if (process == null) throw new ArgumentNullException(nameof(process));
            if (link == null) throw new ArgumentNullException(nameof(link));
            if (output == null) throw new ArgumentNullException(nameof(output));
            var exePath = Directory.GetCurrentDirectory() + "\\ffmpeg.exe";
            //var vi = Directory.GetCurrentDirectory() + "\\test.mp4";
            //vi = "https://s3.amazonaws.com/com.tastemade.tma-originals/animal-hotcake-pops_l_en.mp4";
            //var vo = Directory.GetCurrentDirectory() + "\\test_Out.mp4";
            //vo = @"D:\Temp\Datasave\Xpath\Xpath\bin\Debug\Output\animal-hotcake-pops\animal-hotcake-pops_l_en.mp4";
            //org
            //process = Process.Start(exePath, "-y -i " + link + " -vcodec libx264 -pix_fmt yuv420p -r 30 -g 60 -b:v 1400k -profile:v main -level 3.1 -acodec libmp3lame -b:a 128k -ar 44100 -preset superfast \"" + output + "\"");
            //speed 1.7
            process = Process.Start(exePath, "-y -i " + link + "-filter_complex \"setpts=PTS/1.1; atempo=1.1\" -vcodec libx264 -pix_fmt yuv420p -r 30 -g 60 -b:v 1400k -profile:v main -level 3.1 -acodec libmp3lame -b:a 128k -ar 44100 -preset superfast \"" + output + "\"");
            Debug.Assert(process != null, "process != null");
            process.WaitForExit();
            return process;
        }
    }
}