using System;
namespace StoreBox.Models
{
    public class User
    {
        public int ID { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
        public string mail { get; set; }
    }
}
