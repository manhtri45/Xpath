using System;

namespace Xpath
{
    public class Videos
    {
        public Videos(string iD, string name, string link, DateTime time, string catologi)
        {
            Id = iD;
            Name = name;
            Link = link;
            Time = time;
            Catologi = catologi;
        }

        public Videos()
        {
            Id = string.Empty;
            Name = string.Empty;
            Link = string.Empty;
            Time = DateTime.Today;
            Catologi = string.Empty;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public DateTime Time { get; set; }
        public string Catologi { get; set; }
    }
}
