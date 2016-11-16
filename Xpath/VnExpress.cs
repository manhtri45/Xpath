using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Linq;
using HtmlAgilityPack;
using static System.IO.Path;

namespace Xpath
{
    class VnExpress
    {
        public VnExpress(List<XElement> nodes)
        {
            Nodes = nodes;
        }

        public List<XElement> Nodes { get; }

        public static void Vnex()
        {
            var file = Directory.GetCurrentDirectory() + "\\TENNIS.txt";
            file = File.ReadAllText(file);
            var doc = new HtmlDocument();
            doc.LoadHtml(file);
            const string curenPath = "//*[@class='item_video']/h2/a";
            var nodes = doc.DocumentNode.SelectNodes(curenPath);
            var writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\TENNISList.txt");
            foreach (var node in nodes)
            {
                var temp = node.GetAttributeValue("href", string.Empty);
                var video = SaveVideo(temp);
                writer.WriteLine(video);
            }
            writer.Close();
        }

        public static string SaveVideo(string link)
        {
            const string curenPath = "//*[@id='player_playing']";
            try
            {
                var videos = string.Empty;
                string html = null;
                var webClient = new WebClient();
                using (var stream = webClient.OpenRead(new Uri(link)))
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            html = reader.ReadToEnd();
                        }
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var nodes = doc.DocumentNode.SelectNodes(curenPath);
                foreach (var node in nodes)
                {
                    var lines = node.InnerHtml.Split(';');
                    foreach (var line in lines)
                    {
                        if (line.Contains("videoSource"))
                        {
                            if (line.Contains("= false"))
                                continue;
                            var temp = line.Split('\'');
                            videos = temp[1];
                            Console.WriteLine(videos);
                        }

                    }
                }
                return videos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }

        public static void MoveVideo()
        {
            const string path = @"D:\Temp\vnex";
            const string desPath = @"D:\Temp\Clips";
            var list = Directory.GetDirectories(path);
            foreach (var folder in list)
            {
                var files = Directory.GetFiles(folder);
                foreach (var file in files)
                {
                    if (file != null) File.Copy(file, Combine(desPath, GetFileName(file)));
                }
            }
        }

        public static List<VnItem> GetItem()
        {
            var list = new List<VnItem>();
            var file = Directory.GetCurrentDirectory() + "\\TENNIS.txt";
            file = File.ReadAllText(file);
            var doc = new HtmlDocument();
            doc.LoadHtml(file);
            const string curenPath = "//*[@class='item_video']/div/a";
            var nodes = doc.DocumentNode.SelectNodes(curenPath);
            foreach (var node in nodes)
            {
                var item = new VnItem
                {
                    Title = node.GetAttributeValue("title", string.Empty),
                    LinkPost = node.GetAttributeValue("href", string.Empty),
                    Id = node.GetAttributeValue("data-vid", string.Empty)
                };
                list.Add(item);
            }
            return list;
        }

        public static void GetInfoItems()
        {
            var count = 1;
            var list = GetItem();
            var listVideo = new StreamWriter(Directory.GetCurrentDirectory() + "\\ListVideo.txt");
            var listInfo = new StreamWriter(Directory.GetCurrentDirectory() + "\\ListInfo.txt");
            foreach (var item in list)
            {
                var webClient = new WebClient();
                string html = null;
                using (var stream = webClient.OpenRead(new Uri(item.LinkPost)))
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            html = reader.ReadToEnd();
                        }
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var curenPath = "//*[@id='player_playing']";
                var nodes = doc.DocumentNode.SelectNodes(curenPath);
                foreach (var node in nodes)
                {
                    var lines = node.InnerHtml.Split(';');
                    foreach (var line in lines)
                    {
                        if (!line.Contains("videoSource")) continue;
                        if (line.Contains("= false"))
                            continue;
                        var temp = line.Split('\'');
                        item.LinkVideo = temp[1];
                    }
                }

                curenPath = "//*[@class='lead_detail_video width_common space_bottom_10']";
                nodes = doc.DocumentNode.SelectNodes(curenPath);
                foreach (var node in nodes)
                {
                    item.Desc = node.InnerText;
                }

                curenPath = "//*[@class='block_timer left txt_666']";
                nodes = doc.DocumentNode.SelectNodes(curenPath);
                foreach (var node in nodes)
                {
                    var temp = node.LastChild.InnerText.Split(';');
                    item.Date = temp[temp.Length - 1].Trim();
                }
                Console.WriteLine("{0}%", count * 100 / list.Count);
                listInfo.WriteLine(count++);
                listInfo.WriteLine(item.Date);
                listInfo.WriteLine(item.Id);
                listInfo.WriteLine(item.Title);
                listInfo.WriteLine(item.Desc);
                listInfo.WriteLine(item.LinkPost);
                listInfo.WriteLine(item.LinkVideo);
                listVideo.WriteLine(item.LinkVideo);
                listInfo.WriteLine(item.Timer);
                listInfo.WriteLine();

            }
            listInfo.Close();
            listVideo.Close();
        }

        public class VnItem
        {
            public string Id { set; get; }
            public string LinkPost { set; get; }
            public string Title { set; get; }
            public string LinkVideo { set; get; }
            public string LinkThumnail { set; get; }
            public string Timer { set; get; }
            public string Desc { set; get; }
            public string Date { set; get; }
        }
    }
}