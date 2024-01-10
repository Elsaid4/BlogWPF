using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogWPF.Model
{
    public interface IDbWorker
    {
        bool Login(string userName, string pass);
        List<Blog> GetBlogs(string userName);
        List<Post> GetPosts(int blogId);
        string GetContent(int postId);
        string GetTitle(int postId);
        (bool Added, Blog Blog) AddBlog(string blogName, string userName);
        public (bool Added, Post Post) AddPost(string title, string content, int blogId);
        public string GetUserName(string userName);
        public bool DeleteBlog(int blogId);
        public bool DeletePost(int postId);
        public string GetName(string userName);
        public (bool Edited, string Name) EditName(string newName, string username);
        public bool DeleteUser(string userName);
        public (bool Added, User User) AddUser(string userName, string name, string pass);
        public List<Blog> GetBlogByName(string blogName, string username);
        public List<Post> GetPostByTitle(string postTitle, string username);
        public Blog GetBlogByPost(int postId);
        public bool EditPost(int postId, string newTitle, string newContent);
        public bool EditBlog(int blogId, string newBlogName);

    }
}
