namespace MPP_Client.Models
{
    public class User : IIdentifiable<int?>
    {
        public int? Id       { get; set; }
        public string Name     { get; set; }
        public string Email    { get; set; }
        public string Password { get; set; }

        public User(int? id, string name, string email, string password)
        {
            Id       = id;
            Name     = name;
            Email    = email;
            Password = password;
        }

        public User(string name, string email, string password)
            : this(null, name, email, password) { }

        public override string ToString() =>
            $"{Id} {Name} {Email} {Password}";
    }
}