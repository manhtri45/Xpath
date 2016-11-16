using System;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace Xpath
{
    class BikeExif
    {
        private static void Testc(string uri, int start, int end)
        {
            var listLink = string.Empty;
            var webClient = new WebClient();
            //string uri = "http://www.bikeexif.com/";
            var i = 2;
            const string curenPath = "//*[@class='feed']/article";
            for (i = start; i <= end; i++)
            {
                string html;
                using (var stream = webClient.OpenRead(new Uri(uri)))
                using (var reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                //var curenPath = "//*[@class='item_video']";
                var nodes = doc.DocumentNode.SelectNodes(curenPath);
                
                foreach (var node in nodes)
                {
                    var listAtt = node.Attributes;
                    var child = node.ChildNodes[0];
                    if (listAtt[0].Value.Equals("big"))
                        SaveImage(child.GetAttributeValue("href", string.Empty));
                    else if (listAtt[0].Value.Equals("smallhalf"))
                    {
                        listAtt = child.Attributes;
                        if (listAtt[0].Value.Equals("left"))
                            SaveImage(child.ChildNodes[1].GetAttributeValue("href", string.Empty));
                    }
                }
                Console.WriteLine(uri);
                uri = "http://www.bikeexif.com/page/" + i.ToString();
            }
        }

        public static void SaveImage(string link)
        {
            string html = null;
            var webClient = new WebClient();
            var temp = link.Split('/');
            var path = Directory.GetCurrentDirectory() + "\\" + temp[temp.Length - 1];
            if (Directory.Exists(path)) return;
            Directory.CreateDirectory(path);
            var writer = new StreamWriter(path + "\\info.txt");
            writer.WriteLine(link);
            writer.Close();
            using (var stream = webClient.OpenRead(new Uri(link)))
                if (stream != null)
                    using (var reader = new StreamReader(stream))
                    {
                        html = reader.ReadToEnd();
                    }
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var parentNode = doc.GetElementbyId("content");
            var nodes = parentNode.ChildNodes;
            foreach (var node in nodes)
            {
                var child = node;
                if (node.FirstChild.Name.Equals("a"))
                {
                    child = node.FirstChild;
                }
                var linkImg = child.ChildNodes[0].GetAttributeValue("srcset", string.Empty);
                var tt = linkImg.Split(',');
                if (string.IsNullOrEmpty(tt[tt.Length - 1])) continue;
                var img = tt[tt.Length - 1].Substring(1, tt[tt.Length - 1].Length - 6).Trim();
                var imgName = img.Split('/');
                if (!string.IsNullOrEmpty(img)) webClient.DownloadFile(img, path + "\\" + imgName[imgName.Length - 1]);
            }
        }
    }
}