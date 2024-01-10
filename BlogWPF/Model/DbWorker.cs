using OSKHelpers.Logging;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlogWPF.Model
{
    class DbWorker : IDbWorker
    {
        public List<Blog> GetBlogs(string userName)
        {
            var blogs = new List<Blog>();
            SimpleLog.LogLevel = SimpleLog.LogLevel;
            try
            {
                using (var context = new BloggingContext())
                {
                    blogs.AddRange(context.Blogs.Where(b => b.User.Username == userName));
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                blogs.Clear();
            }

            return blogs;
        }

        public string GetTitle(int postId)
        {
            var title = string.Empty;
            try
            {
                using (var context = new BloggingContext())
                {
                    title = context.Posts.Where(p => p.PostId == postId).FirstOrDefault().Title;
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                title = string.Empty;
            }
            return title;
        }

        public string GetContent(int postId)
        {
            var content = string.Empty;
            try
            {
                using (var context = new BloggingContext())
                {
                    content = context.Posts.Where(p => p.PostId == postId).FirstOrDefault().Content;
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                content = string.Empty;
            }
            return content;
        }

        public List<Post> GetPosts(int blogId)
        {
            var posts = new List<Post>();
            try
            {
                using (var context = new BloggingContext())
                {
                    posts.AddRange(context.Posts.Where(p => p.BlogId == blogId));
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                posts.Clear();
            }
            return posts;
        }

        public bool Login(string userName, string pass)
        {
            var login = false;
            try
            {
                using (var context = new BloggingContext())
                {
                    login = context.Users.Any(
                        u => u.Username.Trim().ToUpper() == userName.ToUpper()
                        && u.Password == pass);
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                MessageBox.Show(ex.Message);
                login = false;
            }
            return login;
        }

        public (bool Added, Blog Blog) AddBlog(string userName, string blogName)
        {
            var added = false;
            Blog blog = null;
            try
            {
                using (var context = new BloggingContext())
                {
                    blog = new Blog(blogName, userName, context.Users.Where(u => u.Username == userName).FirstOrDefault());
                    context.Blogs.Add(blog);
                    context.SaveChanges();
                    added = true;
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                added = false;
                blog = null;
            }

            return (added, blog);
        }

        public (bool Added, Post Post) AddPost(string title, string content, int blogId)
        {
            var added = false;
            Post post = null;
            try
            {
                using (var context = new BloggingContext())
                {
                    var blog = context.Blogs.Where(b => b.BlogId == blogId).FirstOrDefault();
                    post = new Post(title, content, blogId, blog);
                    context.Posts.Add(post);
                    context.SaveChanges();
                    added = true;
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                added = false;
                post = null;
            }

            return (added, post);
        }

        public (bool Added, User User) AddUser(string userName, string name, string pass)
        {
            var Added = false;
            User user = null;
            try
            {
                using (var context = new BloggingContext())
                {
                    if (!context.Users.Where(u => u.Username == userName).Any())
                    {
                        user = new User(userName, name, pass);
                        context.Users.Add(user);
                        context.SaveChanges();
                        Added = true;
                    }
                    else
                    {
                        MessageBox.Show("Username già in uso");
                        Added = false;
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                Added = false;
                user = null;
            }
            return (Added, user);
        }

        public string GetUserName(string userName)
        {
            var usrName = string.Empty;
            try
            {
                using (var context = new BloggingContext())
                {
                    usrName = context.Users.Where(u => u.Username.ToUpper() == userName.Trim().ToUpper()).FirstOrDefault().Username;
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                usrName = string.Empty;
            }
            return usrName;
        }

        public bool DeleteBlog(int blogId)
        {
            var deleted = false;
            try
            {
                using (var context = new BloggingContext())
                {
                    var blog = context.Blogs.Where(b => b.BlogId == blogId).FirstOrDefault();
                    context.Blogs.Remove(blog);
                    context.SaveChanges();
                    deleted = true;
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                deleted = false;
            }
            return deleted;
        }

        public bool DeletePost(int postId)
        {
            var deleted = false;
            try
            {
                using (var context = new BloggingContext())
                {
                    var post = context.Posts.Where(p => p.PostId == postId).FirstOrDefault();
                    context.Posts.Remove(post);
                    context.SaveChanges();
                    deleted = true;
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                deleted = false;
            }
            return deleted;
        }

        public string GetName(string userName)
        {
            var name = string.Empty;
            try
            {
                using (var context = new BloggingContext())
                {
                    name = context.Users.Where(u => u.Username.ToUpper() == userName.Trim().ToUpper()).FirstOrDefault().DisplayName;
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                name = string.Empty;
            }
            return name;
        }

        public (bool Edited, string Name) EditName(string newName, string username)
        {
            var edited = false;
            var name = string.Empty;
            try
            {
                using (var context = new BloggingContext())
                {
                    var user = context.Users.Where(u => u.Username.ToUpper() == username.Trim().ToUpper()).FirstOrDefault();
                    user.DisplayName = newName;
                    context.SaveChanges();
                    edited = true;
                    name = newName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SimpleLog.LogError(ex);
                edited = false;
                name = string.Empty;
            }
            return (edited, name);
        }

        public bool DeleteUser(string userName)
        {
            var deleted = false;
            try
            {
                using (var context = new BloggingContext())
                {
                    var user = context.Users.Where(u => u.Username.ToUpper() == userName.Trim().ToUpper()).FirstOrDefault();
                    var blogs = context.Blogs.Where(b => b.User.Username == userName);
                    if (!blogs.Any())
                    {
                        context.Users.Remove(user);
                        context.SaveChanges();
                        deleted = true;
                    }
                    else
                    {
                        MessageBox.Show("Utente con blog");
                        deleted = false;
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                deleted = false;
            }
            return deleted;
        }

        public List<Blog> GetBlogByName(string blogName, string username)
        {
            return GetBlogs(username).Where(b => b.Name.ToUpper().Contains(blogName.ToUpper().Trim())).ToList();
        }

        public List<Post> GetPostByTitle(string postTitle, string username)
        {
            var posts = new List<Post>();
            try
            {
                var blogs = GetBlogs(username);
                using (var context = new BloggingContext())
                {
                    foreach (var blog in blogs)
                    {
                        var post = GetPosts(blog.BlogId).Where(p => p.Title.ToUpper().Contains(postTitle.ToUpper().Trim())).ToList();
                        posts.AddRange(post);
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                posts.Clear();
            }
            return posts;
        }

        public Blog GetBlogByPost(int blogId)
        {
            var blog = new Blog();
            try
            {
                using (var context = new BloggingContext())
                {
                    blog = context.Blogs.Where(b => b.BlogId == blogId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                SimpleLog.LogError(ex);
                blog = null;
            }
            return blog;
        }

        public bool EditPost(int postId, string newTitle, string newContent)
        {
            var edited = false;
            try
            {
                using (var context = new BloggingContext())
                {
                    var post = context.Posts.Where(p => p.PostId == postId).FirstOrDefault();
                    post.Title = newTitle;
                    post.Content = newContent;
                    context.SaveChanges();
                    edited = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SimpleLog.LogError(ex);
                edited = false;
            }
            return edited;
        }

        public bool EditBlog(int blogId, string newBlogName)
        {
            var edited = false;
            try
            {
                using (var context = new BloggingContext())
                {
                    var blog = context.Blogs.Where(b => b.BlogId == blogId).FirstOrDefault();
                    blog.Name = newBlogName;
                    context.SaveChanges();
                    edited = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SimpleLog.LogError(ex);
                edited = false;
            }
            return edited;
        }
    }
}
