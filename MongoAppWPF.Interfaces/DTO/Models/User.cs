namespace MongoAppWPF.Interfaces.DTO.Models
{
    public class User
    {
        public  object _id { get; set; }
        public string NickName { get; set; } = "Nick Name";
        public int Age { get; set; } = 0;
        public string Country { get; set; } = "Default Country";
    }
}
