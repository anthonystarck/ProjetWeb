using System;
namespace StoreBox.Models
{
    public class File
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string addingTime { get; set; }
        public int idUser { get; set; }
        public string path { get; set; }
    }
}
