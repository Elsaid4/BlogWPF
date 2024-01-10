using BlogWPF.ViewModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace BlogWPF.Model
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public virtual List<Post> Posts { get; set; }
        [ForeignKey(nameof(Username))]
        public virtual User User { get; set; }
        public string Username { get; set; }

        public Blog()
        {
            Posts   = new List<Post>();
            Name    = string.Empty;
            Url     = string.Empty;
            User    = new User();
        }
        public Blog(int blogId, string name, string url, List<Post> posts, User user)
        {
            BlogId  = blogId;
            Name    = name;
            Url     = url;
            Posts   = posts;
            User    = user;
        }

        public Blog(string name, string userName, User user) : this()
        {
            Name        = name;
            Username    = userName;
            User        = user;
        }

        

    }
}