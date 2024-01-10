using System.ComponentModel.DataAnnotations;

namespace BlogWPF.Model
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public virtual List<Blog> Blogs { get; set; }

        public User()
        {
            Username = string.Empty;
            DisplayName = string.Empty;
            Password = string.Empty;
            Blogs = new List<Blog>();
        }

        public User(string username, string nome, string pass)
        {
            Username = username;
            DisplayName = nome;
            Password = pass;
            Blogs = new List<Blog>();
        }

    }
}
